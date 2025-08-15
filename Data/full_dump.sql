--
-- PostgreSQL database dump
--

-- Dumped from database version 17.4
-- Dumped by pg_dump version 17.4

-- Started on 2025-08-15 17:10:48

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 4 (class 2615 OID 2200)
-- Name: public; Type: SCHEMA; Schema: -; Owner: -
--


CREATE FUNCTION public.check_balance_on_receipt() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF NEW.countresources < 0 THEN
        RAISE EXCEPTION 'Количество ресурсов не может быть отрицательным';
    END IF;
    RETURN NEW;
END;
$$;


--
-- TOC entry 236 (class 1255 OID 57822)
-- Name: check_client_usage(integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.check_client_usage(client_id integer) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1 FROM public.shippingdocument WHERE clientid = client_id
    );
END;
$$;


--
-- TOC entry 237 (class 1255 OID 57823)
-- Name: check_measure_usage(integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.check_measure_usage(measure_id integer) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1 FROM public.balance WHERE measureid = measure_id AND countresources > 0
    ) OR EXISTS (
        SELECT 1 FROM public.receiptresources WHERE measureid = measure_id
    ) OR EXISTS (
        SELECT 1 FROM public.shippingresources WHERE measureid = measure_id
    );
END;
$$;


--
-- TOC entry 235 (class 1255 OID 57821)
-- Name: check_resource_usage(integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.check_resource_usage(res_id integer) RETURNS boolean
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN EXISTS (
        SELECT 1 FROM public.balance WHERE resourcesid = res_id AND countresources > 0
    ) OR EXISTS (
        SELECT 1 FROM public.receiptresources WHERE resourcesid = res_id
    ) OR EXISTS (
        SELECT 1 FROM public.shippingresources WHERE resourcesid = res_id
    );
END;
$$;


--
-- TOC entry 239 (class 1255 OID 57825)
-- Name: soft_delete_client(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.soft_delete_client() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF public.check_client_usage(OLD.id) THEN
        UPDATE public.client SET isdeleted = true WHERE id = OLD.id;
        RETURN NULL;
    END IF;
    RETURN OLD;
END;
$$;


--
-- TOC entry 240 (class 1255 OID 57826)
-- Name: soft_delete_measure(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.soft_delete_measure() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF public.check_measure_usage(OLD.id) THEN
        UPDATE public."measurеs" SET isdeleted = true WHERE id = OLD.id;
        RETURN NULL;
    END IF;
    RETURN OLD;
END;
$$;


--
-- TOC entry 238 (class 1255 OID 57824)
-- Name: soft_delete_resource(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.soft_delete_resource() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF public.check_resource_usage(OLD.id) THEN
        UPDATE public.resources SET isdeleted = true WHERE id = OLD.id;
        RETURN NULL; -- Отменяем удаление
    END IF;
    RETURN OLD; -- Продолжаем удаление, если нет зависимостей
END;
$$;


--
-- TOC entry 254 (class 1255 OID 57775)
-- Name: update_balance_from_receipt(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.update_balance_from_receipt() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- Для операции DELETE
    IF TG_OP = 'DELETE' THEN
        DECLARE
            current_balance INTEGER;
        BEGIN
            SELECT COALESCE(countresources, 0) INTO current_balance
            FROM public.balance
            WHERE resourcesid = OLD.resourcesid AND measureid = OLD.measureid;
            
            IF current_balance < OLD.countresources THEN
                RAISE EXCEPTION 'Нельзя удалить: недостаточно ресурсов (ID: %, доступно: %, требуется: %)', 
                    OLD.resourcesid, current_balance, OLD.countresources;
            END IF;
        END;
        
        UPDATE public.balance
        SET countresources = countresources - OLD.countresources
        WHERE resourcesid = OLD.resourcesid AND measureid = OLD.measureid;
        
        DELETE FROM public.balance WHERE countresources <= 0;
        RETURN OLD;
    END IF;
    
    -- Для операции INSERT
    IF TG_OP = 'INSERT' THEN
        IF NEW.countresources <= 0 THEN
            RAISE EXCEPTION 'Количество ресурсов должно быть положительным';
        END IF;
        
        INSERT INTO public.balance (resourcesid, measureid, countresources)
        VALUES (NEW.resourcesid, NEW.measureid, NEW.countresources)
        ON CONFLICT (resourcesid, measureid)
        DO UPDATE SET countresources = balance.countresources + EXCLUDED.countresources;
        
        RETURN NEW;
    END IF;
    
    -- Для операции UPDATE
    IF TG_OP = 'UPDATE' THEN
       IF (NEW.countresources - OLD.countresources) < 0 AND 
           (SELECT countresources FROM balance 
            WHERE resourcesid = OLD.resourcesid AND measureid = OLD.measureid) < (OLD.countresources - NEW.countresources) THEN
            RAISE EXCEPTION 'Недостаточно ресурсов для уменьшения баланса';
        END IF;
        
        -- Если изменился ресурс или мера
        IF NEW.resourcesid != OLD.resourcesid OR NEW.measureid != OLD.measureid THEN
            -- Уменьшаем старый баланс с защитой от отрицательных значений
            UPDATE public.balance
            SET countresources = GREATEST(countresources - OLD.countresources, 0)
            WHERE resourcesid = OLD.resourcesid AND measureid = OLD.measureid;
            
            -- Увеличиваем новый баланс через UPSERT
            INSERT INTO public.balance (resourcesid, measureid, countresources)
            VALUES (NEW.resourcesid, NEW.measureid, NEW.countresources)
            ON CONFLICT (resourcesid, measureid)
            DO UPDATE SET countresources = balance.countresources + EXCLUDED.countresources;
        ELSE
            -- Если изменилось только количество
            UPDATE public.balance
            SET countresources = GREATEST(countresources + (NEW.countresources - OLD.countresources), 0)
            WHERE resourcesid = NEW.resourcesid AND measureid = NEW.measureid;
        END IF;
        
        -- Очищаем нулевые балансы
        DELETE FROM public.balance WHERE countresources <= 0;
        RETURN NEW;
    END IF;
    
    RETURN NULL;
END;
$$;


--
-- TOC entry 253 (class 1255 OID 41337)
-- Name: update_balance_from_shipping_helper(integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.update_balance_from_shipping_helper(res_id integer) RETURNS void
    LANGUAGE plpgsql
    AS $$
DECLARE
    rec RECORD;
BEGIN
    SELECT * INTO rec FROM shippingresources WHERE id = res_id;
    PERFORM update_balance_from_shipping(rec);
END;
$$;


--
-- TOC entry 255 (class 1255 OID 57772)
-- Name: update_balance_on_status_change(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.update_balance_on_status_change() RETURNS trigger
    LANGUAGE plpgsql
    AS $$DECLARE
    resource_record RECORD;
    current_balance INTEGER;
    balance_exists BOOLEAN;
BEGIN
    -- Для статуса "Подписан" (1)
    IF NEW.statusid = 1 AND (TG_OP = 'INSERT' OR OLD.statusid != 1) THEN
        -- Проверяем достаточно ли ресурсов перед списанием
        FOR resource_record IN 
            SELECT sr.resourcesid, sr.measureid, sr.count 
            FROM public.shippingresources sr 
            WHERE sr.documentid = NEW.id
        LOOP
            -- Проверяем существование записи в балансе
            SELECT EXISTS (
                SELECT 1 FROM public.balance
                WHERE resourcesid = resource_record.resourcesid
                AND measureid = resource_record.measureid
            ) INTO balance_exists;
            
            IF NOT balance_exists THEN
                RAISE EXCEPTION 'Ресурс % (мера: %) отсутствует на складе', 
                    resource_record.resourcesid, 
                    resource_record.measureid;
            END IF;
            
            -- Получаем текущий баланс
            SELECT countresources INTO current_balance
            FROM public.balance
            WHERE resourcesid = resource_record.resourcesid
            AND measureid = resource_record.measureid;
            
            -- Проверяем достаточно ли ресурсов
            IF current_balance < resource_record.count THEN
                RAISE EXCEPTION 'Недостаточно ресурсов (ID: %, доступно: %, требуется: %)', 
                    resource_record.resourcesid, 
                    current_balance, 
                    resource_record.count;
            END IF;
        END LOOP;
        
        -- Если все проверки пройдены - списываем
        UPDATE public.balance b
        SET countresources = countresources - sr.count
        FROM public.shippingresources sr
        WHERE sr.documentid = NEW.id
        AND b.resourcesid = sr.resourcesid
        AND b.measureid = sr.measureid;
        
        DELETE FROM public.balance WHERE countresources <= 0;
    END IF;
    
    -- Для статуса "Отозван" (2) только если был "Подписан" (1)
    IF NEW.statusid = 2 AND OLD.statusid = 1 THEN
        -- Возвращаем ресурсы
        UPDATE public.balance b
        SET countresources = countresources + sr.count
        FROM public.shippingresources sr
        WHERE sr.documentid = NEW.id
        AND b.resourcesid = sr.resourcesid
        AND b.measureid = sr.measureid;
        
        INSERT INTO public.balance (resourcesid, measureid, countresources)
        SELECT sr.resourcesid, sr.measureid, sr.count
        FROM public.shippingresources sr
        WHERE sr.documentid = NEW.id
        AND NOT EXISTS (
            SELECT 1 FROM public.balance b
            WHERE b.resourcesid = sr.resourcesid
            AND b.measureid = sr.measureid
        );
    END IF;
    
    RETURN NEW;
END;$$;


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 224 (class 1259 OID 33182)
-- Name: balance; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.balance (
    id integer NOT NULL,
    resourcesid integer NOT NULL,
    measureid integer NOT NULL,
    countresources integer NOT NULL
);


--
-- TOC entry 223 (class 1259 OID 33181)
-- Name: balance_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.balance_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4896 (class 0 OID 0)
-- Dependencies: 223
-- Name: balance_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.balance_id_seq OWNED BY public.balance.id;


--
-- TOC entry 222 (class 1259 OID 33175)
-- Name: client; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.client (
    id integer NOT NULL,
    clientname character varying(100) NOT NULL,
    address character varying(100) NOT NULL,
    isdeleted boolean NOT NULL
);


--
-- TOC entry 221 (class 1259 OID 33174)
-- Name: client_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.client_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4897 (class 0 OID 0)
-- Dependencies: 221
-- Name: client_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.client_id_seq OWNED BY public.client.id;


--
-- TOC entry 220 (class 1259 OID 33168)
-- Name: measurеs; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."measurеs" (
    id integer NOT NULL,
    measurename character varying(15) NOT NULL,
    isdeleted boolean NOT NULL
);


--
-- TOC entry 219 (class 1259 OID 33167)
-- Name: measurе_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public."measurе_id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4898 (class 0 OID 0)
-- Dependencies: 219
-- Name: measurе_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public."measurе_id_seq" OWNED BY public."measurеs".id;


--
-- TOC entry 226 (class 1259 OID 33201)
-- Name: receiptdocumet; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.receiptdocumet (
    id integer NOT NULL,
    numberdocument character varying(100) NOT NULL,
    date date NOT NULL
);


--
-- TOC entry 225 (class 1259 OID 33200)
-- Name: receiptdocumet_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.receiptdocumet_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4899 (class 0 OID 0)
-- Dependencies: 225
-- Name: receiptdocumet_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.receiptdocumet_id_seq OWNED BY public.receiptdocumet.id;


--
-- TOC entry 228 (class 1259 OID 33208)
-- Name: receiptresources; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.receiptresources (
    id integer NOT NULL,
    resourcesid integer NOT NULL,
    measureid integer NOT NULL,
    documentid integer NOT NULL,
    countresources integer NOT NULL
);


--
-- TOC entry 227 (class 1259 OID 33207)
-- Name: receiptresources_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.receiptresources_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4900 (class 0 OID 0)
-- Dependencies: 227
-- Name: receiptresources_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.receiptresources_id_seq OWNED BY public.receiptresources.id;


--
-- TOC entry 218 (class 1259 OID 33161)
-- Name: resources; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.resources (
    id integer NOT NULL,
    productname character varying(100) NOT NULL,
    isdeleted boolean NOT NULL
);


--
-- TOC entry 217 (class 1259 OID 33160)
-- Name: resources_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.resources_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4901 (class 0 OID 0)
-- Dependencies: 217
-- Name: resources_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.resources_id_seq OWNED BY public.resources.id;


--
-- TOC entry 230 (class 1259 OID 33230)
-- Name: shippingdocument; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.shippingdocument (
    id integer NOT NULL,
    documentnumber character varying(100) NOT NULL,
    clientid integer NOT NULL,
    date date NOT NULL,
    statusid integer
);


--
-- TOC entry 229 (class 1259 OID 33229)
-- Name: shippingdocument_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.shippingdocument_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4902 (class 0 OID 0)
-- Dependencies: 229
-- Name: shippingdocument_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.shippingdocument_id_seq OWNED BY public.shippingdocument.id;


--
-- TOC entry 234 (class 1259 OID 33254)
-- Name: shippingresources; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.shippingresources (
    id integer NOT NULL,
    resourcesid integer NOT NULL,
    measureid integer NOT NULL,
    documentid integer NOT NULL,
    count integer NOT NULL
);


--
-- TOC entry 233 (class 1259 OID 33253)
-- Name: shippingresources_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.shippingresources_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4903 (class 0 OID 0)
-- Dependencies: 233
-- Name: shippingresources_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.shippingresources_id_seq OWNED BY public.shippingresources.id;


--
-- TOC entry 232 (class 1259 OID 33242)
-- Name: status; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.status (
    id integer NOT NULL,
    statusname character varying(25) NOT NULL
);


--
-- TOC entry 231 (class 1259 OID 33241)
-- Name: status_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.status_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4904 (class 0 OID 0)
-- Dependencies: 231
-- Name: status_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.status_id_seq OWNED BY public.status.id;


--
-- TOC entry 4694 (class 2604 OID 33185)
-- Name: balance id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.balance ALTER COLUMN id SET DEFAULT nextval('public.balance_id_seq'::regclass);


--
-- TOC entry 4693 (class 2604 OID 33178)
-- Name: client id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.client ALTER COLUMN id SET DEFAULT nextval('public.client_id_seq'::regclass);


--
-- TOC entry 4692 (class 2604 OID 33171)
-- Name: measurеs id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."measurеs" ALTER COLUMN id SET DEFAULT nextval('public."measurе_id_seq"'::regclass);


--
-- TOC entry 4695 (class 2604 OID 33204)
-- Name: receiptdocumet id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.receiptdocumet ALTER COLUMN id SET DEFAULT nextval('public.receiptdocumet_id_seq'::regclass);


--
-- TOC entry 4696 (class 2604 OID 33211)
-- Name: receiptresources id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.receiptresources ALTER COLUMN id SET DEFAULT nextval('public.receiptresources_id_seq'::regclass);


--
-- TOC entry 4691 (class 2604 OID 33164)
-- Name: resources id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.resources ALTER COLUMN id SET DEFAULT nextval('public.resources_id_seq'::regclass);


--
-- TOC entry 4697 (class 2604 OID 33233)
-- Name: shippingdocument id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.shippingdocument ALTER COLUMN id SET DEFAULT nextval('public.shippingdocument_id_seq'::regclass);


--
-- TOC entry 4699 (class 2604 OID 33257)
-- Name: shippingresources id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.shippingresources ALTER COLUMN id SET DEFAULT nextval('public.shippingresources_id_seq'::regclass);


--
-- TOC entry 4698 (class 2604 OID 33245)
-- Name: status id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.status ALTER COLUMN id SET DEFAULT nextval('public.status_id_seq'::regclass);


--
-- TOC entry 4713 (class 2606 OID 33187)
-- Name: balance balance_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.balance
    ADD CONSTRAINT balance_pkey PRIMARY KEY (id);


--
-- TOC entry 4715 (class 2606 OID 57802)
-- Name: balance balance_resources_measure_unique; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.balance
    ADD CONSTRAINT balance_resources_measure_unique UNIQUE (resourcesid, measureid);


--
-- TOC entry 4709 (class 2606 OID 33180)
-- Name: client client_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.client
    ADD CONSTRAINT client_pkey PRIMARY KEY (id);


--
-- TOC entry 4711 (class 2606 OID 41327)
-- Name: client clientuniq; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.client
    ADD CONSTRAINT clientuniq UNIQUE (clientname);


--
-- TOC entry 4717 (class 2606 OID 41329)
-- Name: receiptdocumet docuniq; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.receiptdocumet
    ADD CONSTRAINT docuniq UNIQUE (numberdocument);


--
-- TOC entry 4705 (class 2606 OID 41325)
-- Name: measurеs measuresuniq; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."measurеs"
    ADD CONSTRAINT measuresuniq UNIQUE (measurename);


--
-- TOC entry 4707 (class 2606 OID 33173)
-- Name: measurеs measurе_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."measurеs"
    ADD CONSTRAINT "measurе_pkey" PRIMARY KEY (id);


--
-- TOC entry 4719 (class 2606 OID 33206)
-- Name: receiptdocumet receiptdocumet_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.receiptdocumet
    ADD CONSTRAINT receiptdocumet_pkey PRIMARY KEY (id);


--
-- TOC entry 4721 (class 2606 OID 33213)
-- Name: receiptresources receiptresources_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.receiptresources
    ADD CONSTRAINT receiptresources_pkey PRIMARY KEY (id);


--
-- TOC entry 4701 (class 2606 OID 33166)
-- Name: resources resources_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.resources
    ADD CONSTRAINT resources_pkey PRIMARY KEY (id);


--
-- TOC entry 4723 (class 2606 OID 41331)
-- Name: shippingdocument shipdocuniq; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.shippingdocument
    ADD CONSTRAINT shipdocuniq UNIQUE (documentnumber);


--
-- TOC entry 4725 (class 2606 OID 33235)
-- Name: shippingdocument shippingdocument_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.shippingdocument
    ADD CONSTRAINT shippingdocument_pkey PRIMARY KEY (id);


--
-- TOC entry 4729 (class 2606 OID 33259)
-- Name: shippingresources shippingresources_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.shippingresources
    ADD CONSTRAINT shippingresources_pkey PRIMARY KEY (id);


--
-- TOC entry 4727 (class 2606 OID 33247)
-- Name: status status_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.status
    ADD CONSTRAINT status_pkey PRIMARY KEY (id);


--
-- TOC entry 4703 (class 2606 OID 33199)
-- Name: resources uniqname; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.resources
    ADD CONSTRAINT uniqname UNIQUE (productname);


--
-- TOC entry 4743 (class 2620 OID 57776)
-- Name: receiptresources receiptresource_balance_trigger; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER receiptresource_balance_trigger AFTER INSERT OR DELETE OR UPDATE ON public.receiptresources FOR EACH ROW EXECUTE FUNCTION public.update_balance_from_receipt();


--
-- TOC entry 4742 (class 2620 OID 57828)
-- Name: client trg_soft_delete_client; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_soft_delete_client BEFORE DELETE ON public.client FOR EACH ROW EXECUTE FUNCTION public.soft_delete_client();


--
-- TOC entry 4741 (class 2620 OID 57829)
-- Name: measurеs trg_soft_delete_measure; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_soft_delete_measure BEFORE DELETE ON public."measurеs" FOR EACH ROW EXECUTE FUNCTION public.soft_delete_measure();


--
-- TOC entry 4740 (class 2620 OID 57827)
-- Name: resources trg_soft_delete_resource; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_soft_delete_resource BEFORE DELETE ON public.resources FOR EACH ROW EXECUTE FUNCTION public.soft_delete_resource();


--
-- TOC entry 4744 (class 2620 OID 57843)
-- Name: shippingdocument trg_update_balance_on_status_change; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trg_update_balance_on_status_change AFTER INSERT OR UPDATE OF statusid ON public.shippingdocument FOR EACH ROW EXECUTE FUNCTION public.update_balance_on_status_change();


--
-- TOC entry 4735 (class 2606 OID 33236)
-- Name: shippingdocument clientid_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.shippingdocument
    ADD CONSTRAINT clientid_fk FOREIGN KEY (clientid) REFERENCES public.client(id);


--
-- TOC entry 4732 (class 2606 OID 57833)
-- Name: receiptresources documentid_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.receiptresources
    ADD CONSTRAINT documentid_fk FOREIGN KEY (documentid) REFERENCES public.receiptdocumet(id) ON DELETE CASCADE;


--
-- TOC entry 4737 (class 2606 OID 57838)
-- Name: shippingresources documentid_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.shippingresources
    ADD CONSTRAINT documentid_fk FOREIGN KEY (documentid) REFERENCES public.shippingdocument(id) ON DELETE CASCADE;


--
-- TOC entry 4730 (class 2606 OID 33193)
-- Name: balance measureid_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.balance
    ADD CONSTRAINT measureid_fk FOREIGN KEY (measureid) REFERENCES public."measurеs"(id) NOT VALID;


--
-- TOC entry 4733 (class 2606 OID 33219)
-- Name: receiptresources measureid_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.receiptresources
    ADD CONSTRAINT measureid_fk FOREIGN KEY (measureid) REFERENCES public."measurеs"(id);


--
-- TOC entry 4738 (class 2606 OID 33265)
-- Name: shippingresources measureid_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.shippingresources
    ADD CONSTRAINT measureid_fk FOREIGN KEY (measureid) REFERENCES public."measurеs"(id);


--
-- TOC entry 4731 (class 2606 OID 33188)
-- Name: balance resourcesid_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.balance
    ADD CONSTRAINT resourcesid_fk FOREIGN KEY (resourcesid) REFERENCES public.resources(id);


--
-- TOC entry 4734 (class 2606 OID 33214)
-- Name: receiptresources resourcesid_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.receiptresources
    ADD CONSTRAINT resourcesid_fk FOREIGN KEY (resourcesid) REFERENCES public.resources(id);


--
-- TOC entry 4739 (class 2606 OID 33260)
-- Name: shippingresources resourcesid_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.shippingresources
    ADD CONSTRAINT resourcesid_fk FOREIGN KEY (resourcesid) REFERENCES public.resources(id);


--
-- TOC entry 4736 (class 2606 OID 33248)
-- Name: shippingdocument statusid_fk; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.shippingdocument
    ADD CONSTRAINT statusid_fk FOREIGN KEY (statusid) REFERENCES public.status(id) NOT VALID;

INSERT INTO public.status (statusname) VALUES 
('Подписан'),
('Отозван'),
('В ожидании');
-- Completed on 2025-08-15 17:10:48

--
-- PostgreSQL database dump complete
--


using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TestTaskAPI.Model;

namespace TestTaskAPI.Data;

public partial class TesttaskdbContext : DbContext
{
    public TesttaskdbContext()
    {
    }

    public TesttaskdbContext(DbContextOptions<TesttaskdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Balance> Balances { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Measurе> Measurеs { get; set; }

    public virtual DbSet<ReceiptDocumet> Receiptdocumets { get; set; }

    public virtual DbSet<ReceiptResource> Receiptresources { get; set; }

    public virtual DbSet<Resource> Resources { get; set; }

    public virtual DbSet<ShippingDocument> Shippingdocuments { get; set; }

    public virtual DbSet<ShippingResource> Shippingresources { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=123;Username=postgres;Password=Asdfg123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Balance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("balance_pkey");

            entity.ToTable("balance");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Countresources).HasColumnName("countresources");
            entity.Property(e => e.Measureid).HasColumnName("measureid");
            entity.Property(e => e.Resourcesid).HasColumnName("resourcesid");

            entity.HasOne(d => d.Measure).WithMany(p => p.Balances)
                .HasForeignKey(d => d.Measureid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("measureid_fk");

            entity.HasOne(d => d.Resources).WithMany(p => p.Balances)
                .HasForeignKey(d => d.Resourcesid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("resourcesid_fk");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("client_pkey");

            entity.ToTable("client");

            entity.HasIndex(e => e.Clientname, "clientuniq").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.Clientname)
                .HasMaxLength(100)
                .HasColumnName("clientname");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
        });

        modelBuilder.Entity<Measurе>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("measurе_pkey");

            entity.ToTable("measurеs");

            entity.HasIndex(e => e.Measurename, "measuresuniq").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('\"measurе_id_seq\"'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Measurename)
                .HasMaxLength(15)
                .HasColumnName("measurename");
        });

        modelBuilder.Entity<ReceiptDocumet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("receiptdocumet_pkey");

            entity.ToTable("receiptdocumet");

            entity.HasIndex(e => e.Numberdocument, "docuniq").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Numberdocument)
                .HasMaxLength(100)
                .HasColumnName("numberdocument");
        });

        modelBuilder.Entity<ReceiptResource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("receiptresources_pkey");

            entity.ToTable("receiptresources");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Countresources).HasColumnName("countresources");
            entity.Property(e => e.Documentid).HasColumnName("documentid");
            entity.Property(e => e.Measureid).HasColumnName("measureid");
            entity.Property(e => e.Resourcesid).HasColumnName("resourcesid");

            entity.HasOne(d => d.Document).WithMany(p => p.ReceiptResources)
                .HasForeignKey(d => d.Documentid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("documentid_fk");

            entity.HasOne(d => d.Measure).WithMany(p => p.ReceiptResources)
                .HasForeignKey(d => d.Measureid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("measureid_fk");

            entity.HasOne(d => d.Resources).WithMany(p => p.ReceiptResources)
                .HasForeignKey(d => d.Resourcesid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("resourcesid_fk");
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("resources_pkey");

            entity.ToTable("resources");

            entity.HasIndex(e => e.Productname, "uniqname").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Productname)
                .HasMaxLength(100)
                .HasColumnName("productname");
        });

        modelBuilder.Entity<ShippingDocument>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shippingdocument_pkey");

            entity.ToTable("shippingdocument");

            entity.HasIndex(e => e.DocumentNumber, "shipdocuniq").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClientId).HasColumnName("clientid");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.DocumentNumber)
                .HasMaxLength(100)
                .HasColumnName("documentnumber");
            entity.Property(e => e.StatusId).HasColumnName("statusid");

            entity.HasOne(d => d.Client).WithMany(p => p.ShippingDocuments)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("clientid_fk");

            entity.HasOne(d => d.Status).WithMany(p => p.ShippingDocuments)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("statusid_fk");
        });

        modelBuilder.Entity<ShippingResource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shippingresources_pkey");

            entity.ToTable("shippingresources");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.DocumentId).HasColumnName("documentid");
            entity.Property(e => e.MeasureId).HasColumnName("measureid");
            entity.Property(e => e.ResourcesId).HasColumnName("resourcesid");

            entity.HasOne(d => d.Document).WithMany(p => p.ShippingResources)
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("documentid_fk");

            entity.HasOne(d => d.Measure).WithMany(p => p.ShippingResources)
                .HasForeignKey(d => d.MeasureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("measureid_fk");

            entity.HasOne(d => d.Resources).WithMany(p => p.ShippingResources)
                .HasForeignKey(d => d.ResourcesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("resourcesid_fk");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("status_pkey");

            entity.ToTable("status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Statusname)
                .HasMaxLength(25)
                .HasColumnName("statusname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

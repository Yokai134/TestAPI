using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TestTaskAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixedMeasureSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "client",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    clientname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    address = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("client_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "measurеs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    measurename = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("measurе_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "receiptdocumet",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    numberdocument = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("receiptdocumet_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "resources",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    productname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("resources_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "status",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    statusname = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("status_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "balance",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    resourcesid = table.Column<int>(type: "integer", nullable: false),
                    measureid = table.Column<int>(type: "integer", nullable: false),
                    countresources = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("balance_pkey", x => x.id);
                    table.ForeignKey(
                        name: "measureid_fk",
                        column: x => x.measureid,
                        principalTable: "measurеs",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "resourcesid_fk",
                        column: x => x.resourcesid,
                        principalTable: "resources",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "receiptresources",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    resourcesid = table.Column<int>(type: "integer", nullable: false),
                    measureid = table.Column<int>(type: "integer", nullable: false),
                    documentid = table.Column<int>(type: "integer", nullable: false),
                    countresources = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("receiptresources_pkey", x => x.id);
                    table.ForeignKey(
                        name: "documentid_fk",
                        column: x => x.documentid,
                        principalTable: "receiptdocumet",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "measureid_fk",
                        column: x => x.measureid,
                        principalTable: "measurеs",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "resourcesid_fk",
                        column: x => x.resourcesid,
                        principalTable: "resources",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "shippingdocument",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    documentnumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    clientid = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    statusid = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("shippingdocument_pkey", x => x.id);
                    table.ForeignKey(
                        name: "clientid_fk",
                        column: x => x.clientid,
                        principalTable: "client",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "statusid_fk",
                        column: x => x.statusid,
                        principalTable: "status",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "shippingresources",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    resourcesid = table.Column<int>(type: "integer", nullable: false),
                    measureid = table.Column<int>(type: "integer", nullable: false),
                    documentid = table.Column<int>(type: "integer", nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("shippingresources_pkey", x => x.id);
                    table.ForeignKey(
                        name: "documentid_fk",
                        column: x => x.documentid,
                        principalTable: "shippingdocument",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "measureid_fk",
                        column: x => x.measureid,
                        principalTable: "measurеs",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "resourcesid_fk",
                        column: x => x.resourcesid,
                        principalTable: "resources",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_balance_measureid",
                table: "balance",
                column: "measureid");

            migrationBuilder.CreateIndex(
                name: "IX_balance_resourcesid",
                table: "balance",
                column: "resourcesid");

            migrationBuilder.CreateIndex(
                name: "clientuniq",
                table: "client",
                column: "clientname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "measuresuniq",
                table: "measurеs",
                column: "measurename",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "docuniq",
                table: "receiptdocumet",
                column: "numberdocument",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_receiptresources_documentid",
                table: "receiptresources",
                column: "documentid");

            migrationBuilder.CreateIndex(
                name: "IX_receiptresources_measureid",
                table: "receiptresources",
                column: "measureid");

            migrationBuilder.CreateIndex(
                name: "IX_receiptresources_resourcesid",
                table: "receiptresources",
                column: "resourcesid");

            migrationBuilder.CreateIndex(
                name: "uniqname",
                table: "resources",
                column: "productname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_shippingdocument_clientid",
                table: "shippingdocument",
                column: "clientid");

            migrationBuilder.CreateIndex(
                name: "IX_shippingdocument_statusid",
                table: "shippingdocument",
                column: "statusid");

            migrationBuilder.CreateIndex(
                name: "shipdocuniq",
                table: "shippingdocument",
                column: "documentnumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_shippingresources_documentid",
                table: "shippingresources",
                column: "documentid");

            migrationBuilder.CreateIndex(
                name: "IX_shippingresources_measureid",
                table: "shippingresources",
                column: "measureid");

            migrationBuilder.CreateIndex(
                name: "IX_shippingresources_resourcesid",
                table: "shippingresources",
                column: "resourcesid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "balance");

            migrationBuilder.DropTable(
                name: "receiptresources");

            migrationBuilder.DropTable(
                name: "shippingresources");

            migrationBuilder.DropTable(
                name: "receiptdocumet");

            migrationBuilder.DropTable(
                name: "shippingdocument");

            migrationBuilder.DropTable(
                name: "measurеs");

            migrationBuilder.DropTable(
                name: "resources");

            migrationBuilder.DropTable(
                name: "client");

            migrationBuilder.DropTable(
                name: "status");
        }
    }
}

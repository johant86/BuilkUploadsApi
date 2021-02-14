using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace builk_uploads_api.Migrations
{
    public partial class ConfigMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_DataType",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lastModificationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    lastModificationUser = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_DataType", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tb_Source",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lastModificationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    lastModificationUser = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Source", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tb_Validation",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    validation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    validationErrorMsg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lastModificationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    lastModificationUser = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_Validation", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tb_SourceConfiguration",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sourceid = table.Column<int>(type: "int", nullable: true),
                    alias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tableName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    conectionString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sharePointSiteUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sharePointListName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lastModificationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    lastModificationUser = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_SourceConfiguration", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb_SourceConfiguration_tb_Source_Sourceid",
                        column: x => x.Sourceid,
                        principalTable: "tb_Source",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tb_ColumnBySource",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SourceConfigurationid = table.Column<int>(type: "int", nullable: true),
                    filecolumnName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    columnName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Validationid = table.Column<int>(type: "int", nullable: true),
                    DataTypeid = table.Column<int>(type: "int", nullable: true),
                    lastModificationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    lastModificationUser = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_ColumnBySource", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb_ColumnBySource_tb_DataType_DataTypeid",
                        column: x => x.DataTypeid,
                        principalTable: "tb_DataType",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_ColumnBySource_tb_SourceConfiguration_SourceConfigurationid",
                        column: x => x.SourceConfigurationid,
                        principalTable: "tb_SourceConfiguration",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_ColumnBySource_tb_Validation_Validationid",
                        column: x => x.Validationid,
                        principalTable: "tb_Validation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_ColumnBySource_DataTypeid",
                table: "tb_ColumnBySource",
                column: "DataTypeid");

            migrationBuilder.CreateIndex(
                name: "IX_tb_ColumnBySource_SourceConfigurationid",
                table: "tb_ColumnBySource",
                column: "SourceConfigurationid");

            migrationBuilder.CreateIndex(
                name: "IX_tb_ColumnBySource_Validationid",
                table: "tb_ColumnBySource",
                column: "Validationid");

            migrationBuilder.CreateIndex(
                name: "IX_tb_SourceConfiguration_Sourceid",
                table: "tb_SourceConfiguration",
                column: "Sourceid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_ColumnBySource");

            migrationBuilder.DropTable(
                name: "tb_DataType");

            migrationBuilder.DropTable(
                name: "tb_SourceConfiguration");

            migrationBuilder.DropTable(
                name: "tb_Validation");

            migrationBuilder.DropTable(
                name: "tb_Source");
        }
    }
}

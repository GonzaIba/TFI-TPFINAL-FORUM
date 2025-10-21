using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.SQL.Migrations
{
    /// <inheritdoc />
    public partial class ADD_TYC_AND_STATE_COLUMN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "SesionAyuda",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TerminosCondiciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminosCondiciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TerminosCondicionesSesionAyuda",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdTyC = table.Column<int>(type: "int", nullable: false),
                    IDSesion = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Aceptado = table.Column<bool>(type: "bit", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    FechaAceptado = table.Column<DateTime>(type: "datetime2(3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminosCondicionesSesionAyuda", x => new { x.UserId, x.IdTyC, x.IDSesion });
                    table.ForeignKey(
                        name: "FK_TerminosCondicionesSesionAyuda_SesionAyuda_IDSesion",
                        column: x => x.IDSesion,
                        principalTable: "SesionAyuda",
                        principalColumn: "IDSesion");
                    table.ForeignKey(
                        name: "FK_TerminosCondicionesSesionAyuda_TerminosCondiciones_IdTyC",
                        column: x => x.IdTyC,
                        principalTable: "TerminosCondiciones",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TerminosCondicionesSesionAyuda_IDSesion",
                table: "TerminosCondicionesSesionAyuda",
                column: "IDSesion");

            migrationBuilder.CreateIndex(
                name: "IX_TerminosCondicionesSesionAyuda_IdTyC",
                table: "TerminosCondicionesSesionAyuda",
                column: "IdTyC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TerminosCondicionesSesionAyuda");

            migrationBuilder.DropTable(
                name: "TerminosCondiciones");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "SesionAyuda");
        }
    }
}

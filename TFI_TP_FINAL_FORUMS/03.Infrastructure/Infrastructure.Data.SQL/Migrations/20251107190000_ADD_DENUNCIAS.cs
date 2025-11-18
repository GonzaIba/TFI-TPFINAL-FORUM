using System;
using Infrastructure.Data.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.SQL.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20251107190000_ADD_DENUNCIAS")]
    public partial class ADD_DENUNCIAS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Denuncias",
                columns: table => new
                {
                    IDDenuncia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDPublicacion = table.Column<int>(type: "int", nullable: true),
                    IDRespuesta = table.Column<int>(type: "int", nullable: true),
                    IDUsuarioReporto = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Detalle = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FechaDenuncia = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Denuncias", x => x.IDDenuncia);
                    table.ForeignKey(
                        name: "FK_Denuncias_Publicaciones_IDPublicacion",
                        column: x => x.IDPublicacion,
                        principalTable: "Publicaciones",
                        principalColumn: "IDPublicacion",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Denuncias_Respuestas_IDRespuesta",
                        column: x => x.IDRespuesta,
                        principalTable: "Respuestas",
                        principalColumn: "IDRespuesta",
                        onDelete: ReferentialAction.NoAction);
                    table.CheckConstraint(
                        name: "CK_Denuncias_PublicacionOrRespuesta",
                        sql: "(([IDPublicacion] IS NOT NULL AND [IDRespuesta] IS NULL) OR ([IDPublicacion] IS NULL AND [IDRespuesta] IS NOT NULL))");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Denuncias_IDPublicacion",
                table: "Denuncias",
                column: "IDPublicacion");

            migrationBuilder.CreateIndex(
                name: "IX_Denuncias_IDRespuesta",
                table: "Denuncias",
                column: "IDRespuesta");

            migrationBuilder.CreateIndex(
                name: "IX_Denuncias_IDUsuarioReporto",
                table: "Denuncias",
                column: "IDUsuarioReporto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Denuncias");
        }

        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            new SnapshotBuilder().Build(modelBuilder);
        }

        private sealed class SnapshotBuilder : ApplicationDbContextModelSnapshot
        {
            public void Build(ModelBuilder modelBuilder) => base.BuildModel(modelBuilder);
        }
    }
}

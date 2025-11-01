using Infrastructure.Data.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.SQL.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20251105183000_ADD_SESIONAYUDA_ESTADO")]
    public partial class ADD_SESIONAYUDA_ESTADO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SesionAyudaEstado",
                columns: table => new
                {
                    IDEstado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Estado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SesionAyudaEstado", x => x.IDEstado);
                });

            migrationBuilder.CreateIndex(
                name: "UQ_SesionAyudaEstado_Estado",
                table: "SesionAyudaEstado",
                column: "Estado",
                unique: true);

            migrationBuilder.InsertData(
                table: "SesionAyudaEstado",
                columns: new[] { "IDEstado", "Estado" },
                values: new object[,]
                {
                    { 1, "Pendiente" },
                    { 2, "Iniciada" },
                    { 3, "Finalizada" },
                    { 4, "Cancelada" }
                });

            migrationBuilder.AddColumn<int>(
                name: "IDEstado",
                table: "SesionAyuda",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE SA
SET IDEstado = SE.IDEstado
FROM SesionAyuda AS SA
INNER JOIN SesionAyudaEstado AS SE ON SA.Estado = SE.Estado;
");

            migrationBuilder.Sql(@"
DECLARE @PendienteId INT = (SELECT TOP 1 IDEstado FROM SesionAyudaEstado WHERE Estado = 'Pendiente');
UPDATE SesionAyuda SET IDEstado = @PendienteId WHERE IDEstado IS NULL;
");

            migrationBuilder.AlterColumn<int>(
                name: "IDEstado",
                table: "SesionAyuda",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SesionAyuda_IDEstado",
                table: "SesionAyuda",
                column: "IDEstado");

            migrationBuilder.AddForeignKey(
                name: "FK_SesionAyuda_SesionAyudaEstado_IDEstado",
                table: "SesionAyuda",
                column: "IDEstado",
                principalTable: "SesionAyudaEstado",
                principalColumn: "IDEstado",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "SesionAyuda");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "SesionAyuda",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE SA
SET Estado = SE.Estado
FROM SesionAyuda AS SA
INNER JOIN SesionAyudaEstado AS SE ON SA.IDEstado = SE.IDEstado;
");

            migrationBuilder.Sql(@"
UPDATE SesionAyuda SET Estado = 'Pendiente' WHERE Estado IS NULL OR LTRIM(RTRIM(Estado)) = '';
");

            migrationBuilder.DropForeignKey(
                name: "FK_SesionAyuda_SesionAyudaEstado_IDEstado",
                table: "SesionAyuda");

            migrationBuilder.DropIndex(
                name: "IX_SesionAyuda_IDEstado",
                table: "SesionAyuda");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "SesionAyuda",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "IDEstado",
                table: "SesionAyuda");

            migrationBuilder.DropTable(
                name: "SesionAyudaEstado");
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

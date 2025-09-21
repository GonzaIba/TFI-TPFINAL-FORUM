using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.SQL.Migrations
{
    /// <inheritdoc />
    public partial class LIVE_HELP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitudAyudaEstado",
                columns: table => new
                {
                    IDEstado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Estado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudAyudaEstado", x => x.IDEstado);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudAyuda",
                columns: table => new
                {
                    IDSolicitudAyuda = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Urgencia = table.Column<byte>(type: "tinyint", nullable: false),
                    Lenguaje = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IDUsuarioSolicitante = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IDEstado = table.Column<int>(type: "int", nullable: false),
                    RecompensaBase = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    IncrementoPorHora = table.Column<decimal>(type: "decimal(6,4)", precision: 6, scale: 4, nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudAyuda", x => x.IDSolicitudAyuda);
                    table.ForeignKey(
                        name: "FK_SolicitudAyuda_SolicitudAyudaEstado_IDEstado",
                        column: x => x.IDEstado,
                        principalTable: "SolicitudAyudaEstado",
                        principalColumn: "IDEstado");
                });

            migrationBuilder.CreateTable(
                name: "SolicitudAyudaDisponibilidad",
                columns: table => new
                {
                    IDDisponibilidad = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDSolicitudAyuda = table.Column<int>(type: "int", nullable: false),
                    Inicio = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    Fin = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    Estado = table.Column<byte>(type: "tinyint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudAyudaDisponibilidad", x => x.IDDisponibilidad);
                    table.ForeignKey(
                        name: "FK_SolicitudAyudaDisponibilidad_SolicitudAyuda_IDSolicitudAyuda",
                        column: x => x.IDSolicitudAyuda,
                        principalTable: "SolicitudAyuda",
                        principalColumn: "IDSolicitudAyuda",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudAyudaEtiquetas",
                columns: table => new
                {
                    IDSolicitudAyudaEtiquetas = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDSolicitudAyuda = table.Column<int>(type: "int", nullable: false),
                    IDEtiqueta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudAyudaEtiquetas", x => new { x.IDSolicitudAyudaEtiquetas, x.IDSolicitudAyuda, x.IDEtiqueta });
                    table.ForeignKey(
                        name: "FK_SolicitudAyudaEtiquetas_Etiquetas_IDEtiqueta",
                        column: x => x.IDEtiqueta,
                        principalTable: "Etiquetas",
                        principalColumn: "IDEtiqueta");
                    table.ForeignKey(
                        name: "FK_SolicitudAyudaEtiquetas_SolicitudAyuda_IDSolicitudAyuda",
                        column: x => x.IDSolicitudAyuda,
                        principalTable: "SolicitudAyuda",
                        principalColumn: "IDSolicitudAyuda",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudAyudaHistorial",
                columns: table => new
                {
                    IDHistorial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDSolicitudAyuda = table.Column<int>(type: "int", nullable: false),
                    EstadoAnterior = table.Column<byte>(type: "tinyint", nullable: true),
                    EstadoNuevo = table.Column<byte>(type: "tinyint", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    UserIdAccion = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudAyudaHistorial", x => x.IDHistorial);
                    table.ForeignKey(
                        name: "FK_SolicitudAyudaHistorial_SolicitudAyuda_IDSolicitudAyuda",
                        column: x => x.IDSolicitudAyuda,
                        principalTable: "SolicitudAyuda",
                        principalColumn: "IDSolicitudAyuda");
                });

            migrationBuilder.CreateTable(
                name: "SolicitudAyudaReserva",
                columns: table => new
                {
                    IDReserva = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDDisponibilidad = table.Column<int>(type: "int", nullable: false),
                    IDUsuarioAyudante = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Estado = table.Column<byte>(type: "tinyint", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudAyudaReserva", x => x.IDReserva);
                    table.ForeignKey(
                        name: "FK_SolicitudAyudaReserva_SolicitudAyudaDisponibilidad_IDDisponibilidad",
                        column: x => x.IDDisponibilidad,
                        principalTable: "SolicitudAyudaDisponibilidad",
                        principalColumn: "IDDisponibilidad");
                });

            migrationBuilder.CreateTable(
                name: "SesionAyuda",
                columns: table => new
                {
                    IDSesion = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IDReserva = table.Column<int>(type: "int", nullable: false),
                    Dominio = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NombreSala = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Inicio = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    Fin = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SesionAyuda", x => x.IDSesion);
                    table.ForeignKey(
                        name: "FK_SesionAyuda_SolicitudAyudaReserva_IDReserva",
                        column: x => x.IDReserva,
                        principalTable: "SolicitudAyudaReserva",
                        principalColumn: "IDReserva");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SesionAyuda_Reserva",
                table: "SesionAyuda",
                column: "IDReserva",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_SesionAyuda_Sala",
                table: "SesionAyuda",
                columns: new[] { "Dominio", "NombreSala" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SA_IDEstado",
                table: "SolicitudAyuda",
                column: "IDEstado");

            migrationBuilder.CreateIndex(
                name: "IX_SA_Solicitante",
                table: "SolicitudAyuda",
                column: "IDUsuarioSolicitante");

            migrationBuilder.CreateIndex(
                name: "IX_SA_Vencimiento",
                table: "SolicitudAyuda",
                column: "FechaVencimiento");

            migrationBuilder.CreateIndex(
                name: "IX_SAD_EstadoTiempo",
                table: "SolicitudAyudaDisponibilidad",
                columns: new[] { "Estado", "Inicio" });

            migrationBuilder.CreateIndex(
                name: "IX_SAD_Solicitud",
                table: "SolicitudAyudaDisponibilidad",
                column: "IDSolicitudAyuda");

            migrationBuilder.CreateIndex(
                name: "UQ_SolicitudAyudaEstado_Estado",
                table: "SolicitudAyudaEstado",
                column: "Estado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudAyudaEtiquetas_IDEtiqueta",
                table: "SolicitudAyudaEtiquetas",
                column: "IDEtiqueta");

            migrationBuilder.CreateIndex(
                name: "UX_SolicitudAyudaEtiquetas",
                table: "SolicitudAyudaEtiquetas",
                columns: new[] { "IDSolicitudAyuda", "IDEtiqueta" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SAH_Solicitud",
                table: "SolicitudAyudaHistorial",
                columns: new[] { "IDSolicitudAyuda", "CreateDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SolRes_Ayudante",
                table: "SolicitudAyudaReserva",
                columns: new[] { "IDUsuarioAyudante", "Estado" });

            migrationBuilder.CreateIndex(
                name: "UX_SolRes_Disponibilidad_Activa",
                table: "SolicitudAyudaReserva",
                column: "IDDisponibilidad",
                unique: true,
                filter: "([Estado] IN (0,1,2))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SesionAyuda");

            migrationBuilder.DropTable(
                name: "SolicitudAyudaEtiquetas");

            migrationBuilder.DropTable(
                name: "SolicitudAyudaHistorial");

            migrationBuilder.DropTable(
                name: "SolicitudAyudaReserva");

            migrationBuilder.DropTable(
                name: "SolicitudAyudaDisponibilidad");

            migrationBuilder.DropTable(
                name: "SolicitudAyuda");

            migrationBuilder.DropTable(
                name: "SolicitudAyudaEstado");
        }
    }
}

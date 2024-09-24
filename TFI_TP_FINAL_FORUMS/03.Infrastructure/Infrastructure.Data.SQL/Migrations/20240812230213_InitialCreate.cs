using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.SQL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Etiquetas",
                columns: table => new
                {
                    IDEtiqueta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreEtiqueta = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etiquetas", x => x.IDEtiqueta);
                });

            migrationBuilder.CreateTable(
                name: "Medallas",
                columns: table => new
                {
                    IDMedalla = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreMedalla = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CantidadEntregada = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medallas", x => x.IDMedalla);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    IDNotificacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    FechaNotificacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Leida = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.IDNotificacion);
                });

            migrationBuilder.CreateTable(
                name: "Publicaciones",
                columns: table => new
                {
                    IDPublicacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recompensa = table.Column<int>(type: "int", nullable: false),
                    Visitas = table.Column<int>(type: "int", nullable: false),
                    Respondida = table.Column<bool>(type: "bit", nullable: false),
                    Cerrada = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publicaciones", x => x.IDPublicacion);
                });

            migrationBuilder.CreateTable(
                name: "TextoPredicciones",
                columns: table => new
                {
                    IDTextoPrediccion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Etiquetas = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextoPredicciones", x => x.IDTextoPrediccion);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosMedallas",
                columns: table => new
                {
                    IDUsuarioMedalla = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IDMedalla = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosMedallas", x => x.IDUsuarioMedalla);
                    table.ForeignKey(
                        name: "FK_UsuariosMedallas_Medallas_IDMedalla",
                        column: x => x.IDMedalla,
                        principalTable: "Medallas",
                        principalColumn: "IDMedalla",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EtiquetasPublicaciones",
                columns: table => new
                {
                    IDEtiquetaPublicacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDPublicacion = table.Column<int>(type: "int", nullable: false),
                    IDEtiqueta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtiquetasPublicaciones", x => x.IDEtiquetaPublicacion);
                    table.ForeignKey(
                        name: "FK_EtiquetasPublicaciones_Etiquetas_IDEtiqueta",
                        column: x => x.IDEtiqueta,
                        principalTable: "Etiquetas",
                        principalColumn: "IDEtiqueta",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EtiquetasPublicaciones_Publicaciones_IDPublicacion",
                        column: x => x.IDPublicacion,
                        principalTable: "Publicaciones",
                        principalColumn: "IDPublicacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PublicacionesGuardadas",
                columns: table => new
                {
                    IDPublicacionGuardada = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IDPublicacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicacionesGuardadas", x => x.IDPublicacionGuardada);
                    table.ForeignKey(
                        name: "FK_PublicacionesGuardadas_Publicaciones_IDPublicacion",
                        column: x => x.IDPublicacion,
                        principalTable: "Publicaciones",
                        principalColumn: "IDPublicacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Respuestas",
                columns: table => new
                {
                    IDRespuesta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDPublicacion = table.Column<int>(type: "int", nullable: false),
                    IDUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    TextoRespuesta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    RespuestaCorrecta = table.Column<bool>(type: "bit", nullable: false),
                    Votos = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Respuestas", x => x.IDRespuesta);
                    table.ForeignKey(
                        name: "FK_Respuestas_Publicaciones_IDPublicacion",
                        column: x => x.IDPublicacion,
                        principalTable: "Publicaciones",
                        principalColumn: "IDPublicacion");
                });

            migrationBuilder.CreateTable(
                name: "Archivos",
                columns: table => new
                {
                    IDArchivo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDPublicacion = table.Column<int>(type: "int", nullable: false),
                    IDRespuesta = table.Column<int>(type: "int", nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoArchivo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Archivo = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Archivos", x => x.IDArchivo);
                    table.ForeignKey(
                        name: "FK_Archivos_Publicaciones_IDPublicacion",
                        column: x => x.IDPublicacion,
                        principalTable: "Publicaciones",
                        principalColumn: "IDPublicacion");
                    table.ForeignKey(
                        name: "FK_Archivos_Respuestas_IDRespuesta",
                        column: x => x.IDRespuesta,
                        principalTable: "Respuestas",
                        principalColumn: "IDRespuesta");
                });

            migrationBuilder.CreateTable(
                name: "RecompensasUsuario",
                columns: table => new
                {
                    IDRecompensa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IDRespuesta = table.Column<int>(type: "int", nullable: false),
                    FechaObtencion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    CantidadRecompensa = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecompensasUsuario", x => x.IDRecompensa);
                    table.ForeignKey(
                        name: "FK_RecompensasUsuario_Respuestas_IDRespuesta",
                        column: x => x.IDRespuesta,
                        principalTable: "Respuestas",
                        principalColumn: "IDRespuesta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Archivos_IDPublicacion",
                table: "Archivos",
                column: "IDPublicacion");

            migrationBuilder.CreateIndex(
                name: "IX_Archivos_IDRespuesta",
                table: "Archivos",
                column: "IDRespuesta");

            migrationBuilder.CreateIndex(
                name: "IX_EtiquetasPublicaciones_IDEtiqueta",
                table: "EtiquetasPublicaciones",
                column: "IDEtiqueta");

            migrationBuilder.CreateIndex(
                name: "IX_EtiquetasPublicaciones_IDPublicacion",
                table: "EtiquetasPublicaciones",
                column: "IDPublicacion");

            migrationBuilder.CreateIndex(
                name: "IX_PublicacionesGuardadas_IDPublicacion",
                table: "PublicacionesGuardadas",
                column: "IDPublicacion");

            migrationBuilder.CreateIndex(
                name: "IX_RecompensasUsuario_IDRespuesta",
                table: "RecompensasUsuario",
                column: "IDRespuesta");

            migrationBuilder.CreateIndex(
                name: "IX_Respuestas_IDPublicacion",
                table: "Respuestas",
                column: "IDPublicacion");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosMedallas_IDMedalla",
                table: "UsuariosMedallas",
                column: "IDMedalla");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Archivos");

            migrationBuilder.DropTable(
                name: "EtiquetasPublicaciones");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "PublicacionesGuardadas");

            migrationBuilder.DropTable(
                name: "RecompensasUsuario");

            migrationBuilder.DropTable(
                name: "TextoPredicciones");

            migrationBuilder.DropTable(
                name: "UsuariosMedallas");

            migrationBuilder.DropTable(
                name: "Etiquetas");

            migrationBuilder.DropTable(
                name: "Respuestas");

            migrationBuilder.DropTable(
                name: "Medallas");

            migrationBuilder.DropTable(
                name: "Publicaciones");
        }
    }
}

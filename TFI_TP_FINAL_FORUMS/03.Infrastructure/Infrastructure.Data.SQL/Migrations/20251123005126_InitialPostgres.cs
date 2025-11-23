using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Data.SQL.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Etiquetas",
                columns: table => new
                {
                    IDEtiqueta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreEtiqueta = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DescripcionEtiqueta = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etiquetas", x => x.IDEtiqueta);
                });

            migrationBuilder.CreateTable(
                name: "EtiquetasPrediccionModelo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModelData = table.Column<byte[]>(type: "bytea", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtiquetasPrediccionModelo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Medallas",
                columns: table => new
                {
                    IDMedalla = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NombreMedalla = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CantidadEntregada = table.Column<int>(type: "integer", nullable: false),
                    ImagenMedalla = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medallas", x => x.IDMedalla);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    IDNotificacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IDUsuario = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Mensaje = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    FechaNotificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Leida = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.IDNotificacion);
                });

            migrationBuilder.CreateTable(
                name: "Publicaciones",
                columns: table => new
                {
                    IDPublicacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IDUsuario = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Titulo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Contenido = table.Column<string>(type: "text", nullable: false),
                    Recompensa = table.Column<int>(type: "integer", nullable: false),
                    Visitas = table.Column<int>(type: "integer", nullable: false),
                    Respondida = table.Column<bool>(type: "boolean", nullable: false),
                    Cerrada = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaCierre = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publicaciones", x => x.IDPublicacion);
                });

            migrationBuilder.CreateTable(
                name: "Recompensas",
                columns: table => new
                {
                    IDRecompensa = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Valor = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recompensas", x => x.IDRecompensa);
                });

            migrationBuilder.CreateTable(
                name: "SesionAyudaEstado",
                columns: table => new
                {
                    IDEstado = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Estado = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SesionAyudaEstado", x => x.IDEstado);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudAyudaEstado",
                columns: table => new
                {
                    IDEstado = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Estado = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudAyudaEstado", x => x.IDEstado);
                });

            migrationBuilder.CreateTable(
                name: "TerminosCondiciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminosCondiciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TextoPredicciones",
                columns: table => new
                {
                    IDTextoPrediccion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Texto = table.Column<string>(type: "text", nullable: true),
                    Etiquetas = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextoPredicciones", x => x.IDTextoPrediccion);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosMedallas",
                columns: table => new
                {
                    IDUsuarioMedalla = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IDUsuario = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    IDMedalla = table.Column<int>(type: "integer", nullable: false),
                    FechaObtenido = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    IDEtiquetaPublicacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IDPublicacion = table.Column<int>(type: "integer", nullable: false),
                    IDEtiqueta = table.Column<int>(type: "integer", nullable: false)
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
                    IDPublicacionGuardada = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IDUsuario = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    IDPublicacion = table.Column<int>(type: "integer", nullable: false)
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
                name: "PublicacionesVotos",
                columns: table => new
                {
                    IDPublicacion = table.Column<int>(type: "integer", nullable: false),
                    IDUsuario = table.Column<string>(type: "text", nullable: false),
                    Positivo = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicacionesVotos", x => new { x.IDPublicacion, x.IDUsuario });
                    table.ForeignKey(
                        name: "FK_PublicacionesVotos_Publicaciones_IDPublicacion",
                        column: x => x.IDPublicacion,
                        principalTable: "Publicaciones",
                        principalColumn: "IDPublicacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Respuestas",
                columns: table => new
                {
                    IDRespuesta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IDPublicacion = table.Column<int>(type: "integer", nullable: false),
                    IDUsuario = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    TextoRespuesta = table.Column<string>(type: "text", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    RespuestaCorrecta = table.Column<bool>(type: "boolean", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "RecompensasUsuario",
                columns: table => new
                {
                    IDRecompensaUsuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IDUsuario = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    IDRecompensa = table.Column<int>(type: "integer", nullable: false),
                    FechaObtencion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecompensasUsuario", x => x.IDRecompensaUsuario);
                    table.ForeignKey(
                        name: "FK_RecompensasUsuario_Recompensas_IDRecompensa",
                        column: x => x.IDRecompensa,
                        principalTable: "Recompensas",
                        principalColumn: "IDRecompensa",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudAyuda",
                columns: table => new
                {
                    IDSolicitudAyuda = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Urgencia = table.Column<byte>(type: "smallint", nullable: false),
                    Lenguaje = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IDUsuarioSolicitante = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    IDEstado = table.Column<int>(type: "integer", nullable: false),
                    RecompensaBase = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    IncrementoPorHora = table.Column<decimal>(type: "numeric(6,4)", precision: 6, scale: 4, nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp(3) with time zone", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "Archivos",
                columns: table => new
                {
                    IDArchivo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IDPublicacion = table.Column<int>(type: "integer", nullable: false),
                    IDRespuesta = table.Column<int>(type: "integer", nullable: false),
                    NombreArchivo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TipoArchivo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Archivo = table.Column<byte[]>(type: "bytea", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                name: "Denuncias",
                columns: table => new
                {
                    IDDenuncia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IDPublicacion = table.Column<int>(type: "integer", nullable: true),
                    IDRespuesta = table.Column<int>(type: "integer", nullable: true),
                    IDUsuarioReporto = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Motivo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Detalle = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    FechaDenuncia = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Denuncias", x => x.IDDenuncia);
                    table.CheckConstraint("CK_Denuncias_PublicacionOrRespuesta", "((\"IDPublicacion\" IS NOT NULL AND \"IDRespuesta\" IS NULL) OR (\"IDPublicacion\" IS NULL AND \"IDRespuesta\" IS NOT NULL))");
                    table.ForeignKey(
                        name: "FK_Denuncias_Publicaciones_IDPublicacion",
                        column: x => x.IDPublicacion,
                        principalTable: "Publicaciones",
                        principalColumn: "IDPublicacion",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Denuncias_Respuestas_IDRespuesta",
                        column: x => x.IDRespuesta,
                        principalTable: "Respuestas",
                        principalColumn: "IDRespuesta",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RespuestasVotos",
                columns: table => new
                {
                    IDRespuesta = table.Column<int>(type: "integer", nullable: false),
                    IDUsuario = table.Column<string>(type: "text", nullable: false),
                    Positivo = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespuestasVotos", x => new { x.IDRespuesta, x.IDUsuario });
                    table.ForeignKey(
                        name: "FK_RespuestasVotos_Respuestas_IDRespuesta",
                        column: x => x.IDRespuesta,
                        principalTable: "Respuestas",
                        principalColumn: "IDRespuesta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudAyudaChat",
                columns: table => new
                {
                    IDChat = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IDSolicitudAyuda = table.Column<int>(type: "integer", nullable: false),
                    IDUsuarioAyudante = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudAyudaChat", x => x.IDChat);
                    table.ForeignKey(
                        name: "FK_SolicitudAyudaChat_SolicitudAyuda_IDSolicitudAyuda",
                        column: x => x.IDSolicitudAyuda,
                        principalTable: "SolicitudAyuda",
                        principalColumn: "IDSolicitudAyuda",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudAyudaDisponibilidad",
                columns: table => new
                {
                    IDDisponibilidad = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IDSolicitudAyuda = table.Column<int>(type: "integer", nullable: false),
                    Inicio = table.Column<DateTime>(type: "timestamp(3) with time zone", nullable: false),
                    Fin = table.Column<DateTime>(type: "timestamp(3) with time zone", nullable: false),
                    Estado = table.Column<byte>(type: "smallint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    IDSolicitudAyudaEtiquetas = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IDSolicitudAyuda = table.Column<int>(type: "integer", nullable: false),
                    IDEtiqueta = table.Column<int>(type: "integer", nullable: false)
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
                    IDHistorial = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IDSolicitudAyuda = table.Column<int>(type: "integer", nullable: false),
                    EstadoAnterior = table.Column<byte>(type: "smallint", nullable: true),
                    EstadoNuevo = table.Column<byte>(type: "smallint", nullable: false),
                    Motivo = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    UserIdAccion = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp(3) with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
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
                name: "SolicitudAyudaChatMensaje",
                columns: table => new
                {
                    IDMensaje = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IDChat = table.Column<int>(type: "integer", nullable: false),
                    IDUsuario = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Mensaje = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudAyudaChatMensaje", x => x.IDMensaje);
                    table.ForeignKey(
                        name: "FK_SolicitudAyudaChatMensaje_SolicitudAyudaChat_IDChat",
                        column: x => x.IDChat,
                        principalTable: "SolicitudAyudaChat",
                        principalColumn: "IDChat",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudAyudaChatParticipante",
                columns: table => new
                {
                    IDChat = table.Column<int>(type: "integer", nullable: false),
                    IDUsuario = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Rol = table.Column<byte>(type: "smallint", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudAyudaChatParticipante", x => new { x.IDChat, x.IDUsuario });
                    table.ForeignKey(
                        name: "FK_SolicitudAyudaChatParticipante_SolicitudAyudaChat_IDChat",
                        column: x => x.IDChat,
                        principalTable: "SolicitudAyudaChat",
                        principalColumn: "IDChat",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudAyudaReserva",
                columns: table => new
                {
                    IDReserva = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IDDisponibilidad = table.Column<int>(type: "integer", nullable: false),
                    IDUsuarioAyudante = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Estado = table.Column<byte>(type: "smallint", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudAyudaReserva", x => x.IDReserva);
                    table.ForeignKey(
                        name: "FK_SolicitudAyudaReserva_SolicitudAyudaDisponibilidad_IDDispon~",
                        column: x => x.IDDisponibilidad,
                        principalTable: "SolicitudAyudaDisponibilidad",
                        principalColumn: "IDDisponibilidad");
                });

            migrationBuilder.CreateTable(
                name: "SolicitudAyudaChatMensajeLectura",
                columns: table => new
                {
                    IDMensaje = table.Column<int>(type: "integer", nullable: false),
                    IDUsuario = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudAyudaChatMensajeLectura", x => new { x.IDMensaje, x.IDUsuario });
                    table.ForeignKey(
                        name: "FK_SolicitudAyudaChatMensajeLectura_SolicitudAyudaChatMensaje_~",
                        column: x => x.IDMensaje,
                        principalTable: "SolicitudAyudaChatMensaje",
                        principalColumn: "IDMensaje",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SesionAyuda",
                columns: table => new
                {
                    IDSesion = table.Column<Guid>(type: "uuid", nullable: false),
                    IDReserva = table.Column<int>(type: "integer", nullable: false),
                    IDEstado = table.Column<int>(type: "integer", nullable: false),
                    Dominio = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NombreSala = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Inicio = table.Column<DateTime>(type: "timestamp(3) with time zone", nullable: false),
                    Fin = table.Column<DateTime>(type: "timestamp(3) with time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SesionAyuda", x => x.IDSesion);
                    table.ForeignKey(
                        name: "FK_SesionAyuda_SesionAyudaEstado_IDEstado",
                        column: x => x.IDEstado,
                        principalTable: "SesionAyudaEstado",
                        principalColumn: "IDEstado");
                    table.ForeignKey(
                        name: "FK_SesionAyuda_SolicitudAyudaReserva_IDReserva",
                        column: x => x.IDReserva,
                        principalTable: "SolicitudAyudaReserva",
                        principalColumn: "IDReserva");
                });

            migrationBuilder.CreateTable(
                name: "TerminosCondicionesSesionAyuda",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    IdTyC = table.Column<int>(type: "integer", nullable: false),
                    IDSesion = table.Column<Guid>(type: "uuid", nullable: false),
                    Aceptado = table.Column<bool>(type: "boolean", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    FechaAceptado = table.Column<DateTime>(type: "timestamp(3) with time zone", nullable: false)
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
                name: "IX_Archivos_IDPublicacion",
                table: "Archivos",
                column: "IDPublicacion");

            migrationBuilder.CreateIndex(
                name: "IX_Archivos_IDRespuesta",
                table: "Archivos",
                column: "IDRespuesta");

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
                name: "IX_RecompensasUsuario_IDRecompensa",
                table: "RecompensasUsuario",
                column: "IDRecompensa");

            migrationBuilder.CreateIndex(
                name: "IX_Respuestas_IDPublicacion",
                table: "Respuestas",
                column: "IDPublicacion");

            migrationBuilder.CreateIndex(
                name: "IX_SesionAyuda_IDEstado",
                table: "SesionAyuda",
                column: "IDEstado");

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
                name: "UQ_SesionAyudaEstado_Estado",
                table: "SesionAyudaEstado",
                column: "Estado",
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
                name: "IX_SAChat_Ayudante",
                table: "SolicitudAyudaChat",
                column: "IDUsuarioAyudante");

            migrationBuilder.CreateIndex(
                name: "UQ_SAChat_SolicitudAyudante",
                table: "SolicitudAyudaChat",
                columns: new[] { "IDSolicitudAyuda", "IDUsuarioAyudante" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SAChatMsg_Chat",
                table: "SolicitudAyudaChatMensaje",
                column: "IDChat");

            migrationBuilder.CreateIndex(
                name: "IX_SAChatMsg_ChatDate",
                table: "SolicitudAyudaChatMensaje",
                columns: new[] { "IDChat", "CreateDate" });

            migrationBuilder.CreateIndex(
                name: "IX_SAChatMsgRead_User",
                table: "SolicitudAyudaChatMensajeLectura",
                column: "IDUsuario");

            migrationBuilder.CreateIndex(
                name: "UQ_SCP_ChatRol",
                table: "SolicitudAyudaChatParticipante",
                columns: new[] { "IDChat", "Rol" },
                unique: true);

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
                filter: "(\"Estado\" IN (0,1,2))");

            migrationBuilder.CreateIndex(
                name: "IX_TerminosCondicionesSesionAyuda_IDSesion",
                table: "TerminosCondicionesSesionAyuda",
                column: "IDSesion");

            migrationBuilder.CreateIndex(
                name: "IX_TerminosCondicionesSesionAyuda_IdTyC",
                table: "TerminosCondicionesSesionAyuda",
                column: "IdTyC");

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
                name: "Denuncias");

            migrationBuilder.DropTable(
                name: "EtiquetasPrediccionModelo");

            migrationBuilder.DropTable(
                name: "EtiquetasPublicaciones");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "PublicacionesGuardadas");

            migrationBuilder.DropTable(
                name: "PublicacionesVotos");

            migrationBuilder.DropTable(
                name: "RecompensasUsuario");

            migrationBuilder.DropTable(
                name: "RespuestasVotos");

            migrationBuilder.DropTable(
                name: "SolicitudAyudaChatMensajeLectura");

            migrationBuilder.DropTable(
                name: "SolicitudAyudaChatParticipante");

            migrationBuilder.DropTable(
                name: "SolicitudAyudaEtiquetas");

            migrationBuilder.DropTable(
                name: "SolicitudAyudaHistorial");

            migrationBuilder.DropTable(
                name: "TerminosCondicionesSesionAyuda");

            migrationBuilder.DropTable(
                name: "TextoPredicciones");

            migrationBuilder.DropTable(
                name: "UsuariosMedallas");

            migrationBuilder.DropTable(
                name: "Recompensas");

            migrationBuilder.DropTable(
                name: "Respuestas");

            migrationBuilder.DropTable(
                name: "SolicitudAyudaChatMensaje");

            migrationBuilder.DropTable(
                name: "Etiquetas");

            migrationBuilder.DropTable(
                name: "SesionAyuda");

            migrationBuilder.DropTable(
                name: "TerminosCondiciones");

            migrationBuilder.DropTable(
                name: "Medallas");

            migrationBuilder.DropTable(
                name: "Publicaciones");

            migrationBuilder.DropTable(
                name: "SolicitudAyudaChat");

            migrationBuilder.DropTable(
                name: "SesionAyudaEstado");

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

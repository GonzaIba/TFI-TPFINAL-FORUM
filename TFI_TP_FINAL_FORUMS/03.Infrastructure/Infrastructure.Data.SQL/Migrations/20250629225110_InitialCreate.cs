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
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etiquetas", x => x.IDEtiqueta);
                });

            migrationBuilder.CreateTable(
                name: "EtiquetasPrediccionModelo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModelData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtiquetasPrediccionModelo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Filter",
                columns: table => new
                {
                    IDFilter = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Api = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filter", x => x.IDFilter);
                });

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    IDGroup = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.IDGroup);
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
                    ImagenMedalla = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
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
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
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
                    Titulo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recompensa = table.Column<int>(type: "int", nullable: false),
                    Visitas = table.Column<int>(type: "int", nullable: false),
                    Respondida = table.Column<bool>(type: "bit", nullable: false),
                    Cerrada = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publicaciones", x => x.IDPublicacion);
                });

            migrationBuilder.CreateTable(
                name: "Recompensas",
                columns: table => new
                {
                    IDRecompensa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Valor = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recompensas", x => x.IDRecompensa);
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
                name: "UserFilters",
                columns: table => new
                {
                    IDFilter = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFilters", x => new { x.IDFilter, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserFilters_Filter_IDFilter",
                        column: x => x.IDFilter,
                        principalTable: "Filter",
                        principalColumn: "IDFilter",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupFilters",
                columns: table => new
                {
                    IDGroup = table.Column<int>(type: "int", nullable: false),
                    IDFilter = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupFilters", x => new { x.IDGroup, x.IDFilter });
                    table.ForeignKey(
                        name: "FK_GroupFilters_Filter_IDFilter",
                        column: x => x.IDFilter,
                        principalTable: "Filter",
                        principalColumn: "IDFilter",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupFilters_Group_IDGroup",
                        column: x => x.IDGroup,
                        principalTable: "Group",
                        principalColumn: "IDGroup",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosMedallas",
                columns: table => new
                {
                    IDUsuarioMedalla = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IDMedalla = table.Column<int>(type: "int", nullable: false),
                    FechaObtenido = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "PublicacionesVotos",
                columns: table => new
                {
                    IDPublicacion = table.Column<int>(type: "int", nullable: false),
                    IDUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Positivo = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
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
                    IDRespuesta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDPublicacion = table.Column<int>(type: "int", nullable: false),
                    IDUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    TextoRespuesta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    RespuestaCorrecta = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
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
                name: "RecompensasUsuario",
                columns: table => new
                {
                    IDRecompensaUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IDRecompensa = table.Column<int>(type: "int", nullable: false),
                    FechaObtencion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
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
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
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
                name: "RespuestasVotos",
                columns: table => new
                {
                    IDRespuesta = table.Column<int>(type: "int", nullable: false),
                    IDUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Positivo = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
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
                name: "IX_GroupFilters_IDFilter",
                table: "GroupFilters",
                column: "IDFilter");

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
                name: "EtiquetasPrediccionModelo");

            migrationBuilder.DropTable(
                name: "EtiquetasPublicaciones");

            migrationBuilder.DropTable(
                name: "GroupFilters");

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
                name: "TextoPredicciones");

            migrationBuilder.DropTable(
                name: "UserFilters");

            migrationBuilder.DropTable(
                name: "UsuariosMedallas");

            migrationBuilder.DropTable(
                name: "Etiquetas");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropTable(
                name: "Recompensas");

            migrationBuilder.DropTable(
                name: "Respuestas");

            migrationBuilder.DropTable(
                name: "Filter");

            migrationBuilder.DropTable(
                name: "Medallas");

            migrationBuilder.DropTable(
                name: "Publicaciones");
        }
    }
}

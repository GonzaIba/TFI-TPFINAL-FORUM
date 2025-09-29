using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.SQL.Migrations
{
    /// <inheritdoc />
    public partial class ADD_TABLES_CHATS_RH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitudAyudaChat",
                columns: table => new
                {
                    IDChat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDSolicitudAyuda = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "SolicitudAyudaChatMensaje",
                columns: table => new
                {
                    IDMensaje = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDChat = table.Column<int>(type: "int", nullable: false),
                    IDUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()"),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "SolicitudAyudaChatMensajeLectura",
                columns: table => new
                {
                    IDMensaje = table.Column<int>(type: "int", nullable: false),
                    IDUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudAyudaChatMensajeLectura", x => new { x.IDMensaje, x.IDUsuario });
                    table.ForeignKey(
                        name: "FK_SolicitudAyudaChatMensajeLectura_SolicitudAyudaChatMensaje_IDMensaje",
                        column: x => x.IDMensaje,
                        principalTable: "SolicitudAyudaChatMensaje",
                        principalColumn: "IDMensaje",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UQ_SAChat_Solicitud",
                table: "SolicitudAyudaChat",
                column: "IDSolicitudAyuda",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudAyudaChatMensajeLectura");

            migrationBuilder.DropTable(
                name: "SolicitudAyudaChatMensaje");

            migrationBuilder.DropTable(
                name: "SolicitudAyudaChat");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.SQL.Migrations
{
    /// <inheritdoc />
    public partial class ADD_PARTICIPANTS_CHATS_RH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ_SAChat_Solicitud",
                table: "SolicitudAyudaChat");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "SolicitudAyudaChatMensaje");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "SolicitudAyudaChat");

            migrationBuilder.AddColumn<string>(
                name: "IDUsuarioAyudante",
                table: "SolicitudAyudaChat",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SolicitudAyudaChatParticipante",
                columns: table => new
                {
                    IDChat = table.Column<int>(type: "int", nullable: false),
                    IDUsuario = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Rol = table.Column<byte>(type: "tinyint", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getdate()")
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
                name: "UQ_SCP_ChatRol",
                table: "SolicitudAyudaChatParticipante",
                columns: new[] { "IDChat", "Rol" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitudAyudaChatParticipante");

            migrationBuilder.DropIndex(
                name: "IX_SAChat_Ayudante",
                table: "SolicitudAyudaChat");

            migrationBuilder.DropIndex(
                name: "UQ_SAChat_SolicitudAyudante",
                table: "SolicitudAyudaChat");

            migrationBuilder.DropColumn(
                name: "IDUsuarioAyudante",
                table: "SolicitudAyudaChat");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "SolicitudAyudaChatMensaje",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "SolicitudAyudaChat",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "UQ_SAChat_Solicitud",
                table: "SolicitudAyudaChat",
                column: "IDSolicitudAyuda",
                unique: true);
        }
    }
}

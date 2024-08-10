using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToPurchaseHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseHistory_Usuario_UsuarioId",
                table: "PurchaseHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseHistory",
                table: "PurchaseHistory");

            migrationBuilder.RenameTable(
                name: "PurchaseHistory",
                newName: "PurchaseHistories");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "PurchaseHistories",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseHistory_UsuarioId",
                table: "PurchaseHistories",
                newName: "IX_PurchaseHistories_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "MaskedCardNumber",
                table: "PurchaseHistories",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseHistories",
                table: "PurchaseHistories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseHistories_Usuario_UserId",
                table: "PurchaseHistories",
                column: "UserId",
                principalTable: "Usuario",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseHistories_Usuario_UserId",
                table: "PurchaseHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseHistories",
                table: "PurchaseHistories");

            migrationBuilder.RenameTable(
                name: "PurchaseHistories",
                newName: "PurchaseHistory");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PurchaseHistory",
                newName: "UsuarioId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseHistories_UserId",
                table: "PurchaseHistory",
                newName: "IX_PurchaseHistory_UsuarioId");

            migrationBuilder.AlterColumn<string>(
                name: "MaskedCardNumber",
                table: "PurchaseHistory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(16)",
                oldMaxLength: 16);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseHistory",
                table: "PurchaseHistory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseHistory_Usuario_UsuarioId",
                table: "PurchaseHistory",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

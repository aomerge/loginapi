using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace loginapi.Migrations
{
    /// <inheritdoc />
    public partial class incrementedidVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VerificationUsers_Users_id_user",
                table: "VerificationUsers");

            migrationBuilder.DropIndex(
                name: "IX_VerificationUsers_id_user",
                table: "VerificationUsers");

            migrationBuilder.RenameColumn(
                name: "id_user",
                table: "VerificationUsers",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "VerificationUsers",
                newName: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationUsers_id_user",
                table: "VerificationUsers",
                column: "id_user");

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationUsers_Users_id_user",
                table: "VerificationUsers",
                column: "id_user",
                principalTable: "Users",
                principalColumn: "id_user");
        }
    }
}

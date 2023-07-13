using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace loginapi.Migrations
{
    /// <inheritdoc />
    public partial class uniqueEmailUserhandler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_user_handle_email",
                table: "Users",
                columns: new[] { "user_handle", "email" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_user_handle_email",
                table: "Users");
        }
    }
}

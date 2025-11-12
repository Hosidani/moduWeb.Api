using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModuWeb.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicateUsersAndAddUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Remove duplicate users, keeping only the first occurrence of each username
            migrationBuilder.Sql(@"
                DELETE FROM ""Users"" 
                WHERE ""Id"" NOT IN (
                    SELECT MIN(""Id"") 
                    FROM ""Users"" 
                    GROUP BY ""UserName""
                );
            ");

            // Step 2: Add unique constraint on UserName
            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            // Step 3: Add unique constraint on Email (recommended)
            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the unique constraints
            migrationBuilder.DropIndex(
                name: "IX_Users_UserName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");
        }
    }
}
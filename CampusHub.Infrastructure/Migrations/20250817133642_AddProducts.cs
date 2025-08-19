using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MeetupPreferences",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "MarketplaceItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceLikes_UserId",
                table: "MarketplaceLikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_UserID",
                table: "MarketplaceItems",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceItems_Users_UserID",
                table: "MarketplaceItems",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceLikes_Users_UserId",
                table: "MarketplaceLikes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceItems_Users_UserID",
                table: "MarketplaceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceLikes_Users_UserId",
                table: "MarketplaceLikes");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceLikes_UserId",
                table: "MarketplaceLikes");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceItems_UserID",
                table: "MarketplaceItems");

            migrationBuilder.DropColumn(
                name: "MeetupPreferences",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "MarketplaceItems");
        }
    }
}

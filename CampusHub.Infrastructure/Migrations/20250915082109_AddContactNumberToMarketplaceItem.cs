using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContactNumberToMarketplaceItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "MarketplaceItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "MarketplaceItems");
        }
    }
}

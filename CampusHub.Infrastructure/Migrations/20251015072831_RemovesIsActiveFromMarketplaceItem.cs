using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovesIsActiveFromMarketplaceItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "MarketplaceItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "MarketplaceItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

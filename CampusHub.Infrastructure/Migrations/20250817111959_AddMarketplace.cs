using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketplaceItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Condition = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SellerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: false),
                    LikesCount = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketplaceLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketplaceItemId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketplaceLikes_MarketplaceItems_MarketplaceItemId",
                        column: x => x.MarketplaceItemId,
                        principalTable: "MarketplaceItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_CreatedDate",
                table: "MarketplaceItems",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_Location",
                table: "MarketplaceItems",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceItems_SellerId",
                table: "MarketplaceItems",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceLikes_MarketplaceItemId_UserId",
                table: "MarketplaceLikes",
                columns: new[] { "MarketplaceItemId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketplaceLikes");

            migrationBuilder.DropTable(
                name: "MarketplaceItems");
        }
    }
}

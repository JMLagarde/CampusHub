using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "ContactNumber", "DateRegistered", "Email", "FullName", "MeetupPreferences", "Password", "ProfilePictureUrl", "ProgramID", "Role", "StudentNumber", "UpdatedAt", "Username", "YearLevelId" },
                values: new object[] { 999, null, new DateTime(2025, 9, 28, 13, 50, 0, 0, DateTimeKind.Utc), "admin@ucc.edu.ph", "System Administrator", "[]", "admin123", null, null, "Admin", null, null, "admin", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 999);
        }
    }
}

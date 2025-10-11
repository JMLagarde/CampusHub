using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CampusHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEventStatusAndBookmarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EventBookmarks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventBookmarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventBookmarks_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventBookmarks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventBookmarks_CreatedAt",
                table: "EventBookmarks",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EventBookmarks_EventId_UserId",
                table: "EventBookmarks",
                columns: new[] { "EventId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventBookmarks_UserId",
                table: "EventBookmarks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventBookmarks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Events");

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "CampusLocation", "CollegeId", "CollegeId1", "CreatedAt", "Description", "EndDate", "ImagePath", "InterestedCount", "Location", "OrganizerId", "Priority", "ProgramEntityId", "ProgramId", "RegistrationDeadline", "StartDate", "Title", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 2, 1, null, new DateTime(2025, 10, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), "Prepare for your upcoming midterm exams with focus and dedication. Aim for an A+! Don't forget to review and study well. Good luck to all students!", new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "midterm-exam.jpg", 400, "Various Classrooms", null, "High", null, 4, null, new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Midterm Exam", "Academic", null },
                    { 2, 1, 1, null, new DateTime(2025, 10, 2, 22, 0, 0, 0, DateTimeKind.Unspecified), "An exciting event hosted by JPIA-UCC South. Oct 7-8, 2025. Accountancy challenges and workshops.", new DateTime(2025, 10, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "mindscape-metrics2.jpg", 300, "UCC South Campus", null, "Medium", null, 1, null, new DateTime(2025, 10, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mindscape Metrics 2: The Ledgerlore Odyssey: Rise of the Crown", "Academic", null },
                    { 3, 1, 6, null, new DateTime(2025, 10, 3, 22, 0, 0, 0, DateTimeKind.Unspecified), "UCC Computer Studies presents CSD Fair 2025: Bringing Youth to Technology Excellence. Oct 8-9, 2025.", new DateTime(2025, 10, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "csd-fair-2025.jpg", 250, "Computer Studies Building", null, "Medium", null, 30, null, new DateTime(2025, 10, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "CSD Fair 2025", "Academic", null }
                });
        }
    }
}

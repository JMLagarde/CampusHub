using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CampusHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CollegeId = table.Column<int>(type: "int", nullable: false),
                    ProgramId = table.Column<int>(type: "int", nullable: true),
                    CampusLocation = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegistrationDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InterestedCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganizerId = table.Column<int>(type: "int", nullable: true),
                    CollegeId1 = table.Column<int>(type: "int", nullable: true),
                    ProgramEntityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Colleges_CollegeId",
                        column: x => x.CollegeId,
                        principalTable: "Colleges",
                        principalColumn: "CollegeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_Colleges_CollegeId1",
                        column: x => x.CollegeId1,
                        principalTable: "Colleges",
                        principalColumn: "CollegeId");
                    table.ForeignKey(
                        name: "FK_Events_Programs_ProgramEntityId",
                        column: x => x.ProgramEntityId,
                        principalTable: "Programs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Events_Programs_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Events_Users_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "CampusLocation", "CollegeId", "CollegeId1", "CreatedAt", "Date", "Description", "ImagePath", "InterestedCount", "Location", "OrganizerId", "Priority", "ProgramEntityId", "ProgramId", "RegistrationDeadline", "Title", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 2, 1, null, new DateTime(2025, 10, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "Prepare for your upcoming midterm exams with focus and dedication. Aim for an A+! Don't forget to review and study well. Good luck to all students!", "midterm-exam.jpg", 400, "Various Classrooms", null, "High", null, 4, null, "Midterm Exam", "Academic", null },
                    { 2, 1, 1, null, new DateTime(2025, 10, 2, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 10, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "An exciting event hosted by JPIA-UCC South. Oct 7-8, 2025. Accountancy challenges and workshops.", "mindscape-metrics2.jpg", 300, "UCC South Campus", null, "Medium", null, 1, null, "Mindscape Metrics 2: The Ledgerlore Odyssey: Rise of the Crown", "Academic", null },
                    { 3, 1, 6, null, new DateTime(2025, 10, 3, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 10, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "UCC Computer Studies presents CSD Fair 2025: Bringing Youth to Technology Excellence. Oct 8-9, 2025.", "csd-fair-2025.jpg", 250, "Computer Studies Building", null, "Medium", null, 30, null, "CSD Fair 2025", "Academic", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_CampusLocation",
                table: "Events",
                column: "CampusLocation");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CollegeId",
                table: "Events",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CollegeId1",
                table: "Events",
                column: "CollegeId1");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CreatedAt",
                table: "Events",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Date",
                table: "Events",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Events_OrganizerId",
                table: "Events",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ProgramEntityId",
                table: "Events",
                column: "ProgramEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ProgramId",
                table: "Events",
                column: "ProgramId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}

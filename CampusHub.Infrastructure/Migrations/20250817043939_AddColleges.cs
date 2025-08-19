using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CampusHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddColleges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Colleges",
                columns: table => new
                {
                    CollegeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollegeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colleges", x => x.CollegeId);
                });

            migrationBuilder.CreateTable(
                name: "YearLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YearLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Programs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CollegeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Programs_Colleges_CollegeId",
                        column: x => x.CollegeId,
                        principalTable: "Colleges",
                        principalColumn: "CollegeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentNumber = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    Username = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateRegistered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearLevelId = table.Column<int>(type: "int", nullable: true),
                    ProgramID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_Users_Programs_ProgramID",
                        column: x => x.ProgramID,
                        principalTable: "Programs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_YearLevels_YearLevelId",
                        column: x => x.YearLevelId,
                        principalTable: "YearLevels",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Colleges",
                columns: new[] { "CollegeId", "CollegeName" },
                values: new object[,]
                {
                    { 1, "College of Business and Accountancy" },
                    { 2, "College of Criminal Justice Education" },
                    { 3, "College of Education" },
                    { 4, "College of Engineering" },
                    { 5, "College of Law" },
                    { 6, "College of Liberal Arts and Sciences" },
                    { 7, "Graduate School" }
                });

            migrationBuilder.InsertData(
                table: "YearLevels",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "1st Year" },
                    { 2, "2nd Year" },
                    { 3, "3rd Year" },
                    { 4, "4th Year" },
                    { 5, "5th Year" },
                    { 6, "Graduate" }
                });

            migrationBuilder.InsertData(
                table: "Programs",
                columns: new[] { "Id", "CollegeId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Bachelor of Science in Accountancy" },
                    { 2, 1, "Bachelor of Science in Accounting Information System" },
                    { 3, 1, "Bachelor of Science in Business Administration, Major in Financial Management" },
                    { 4, 1, "Bachelor of Science in Business Administration, Major in Human Resource Management" },
                    { 5, 1, "Bachelor of Science in Business Administration, Major in Marketing Management" },
                    { 6, 1, "Bachelor of Science in Entrepreneurship" },
                    { 7, 1, "Bachelor of Science in Hospitality Management" },
                    { 8, 1, "Bachelor of Science in Office Administration" },
                    { 9, 1, "Bachelor of Science in Tourism Management" },
                    { 10, 2, "Bachelor of Science in Criminology" },
                    { 11, 3, "Bachelor in Secondary Education Major in English" },
                    { 12, 3, "Bachelor in Secondary Education Major in English - Chinese" },
                    { 13, 3, "Bachelor in Secondary Education Major in Science" },
                    { 14, 3, "Bachelor in Secondary Education Major in Technology and Livelihood Education" },
                    { 15, 3, "Bachelor of Early Childhood Education" },
                    { 16, 3, "Certificate in Professional Education" },
                    { 17, 3, "Elementary | Secondary | P.E." },
                    { 18, 4, "Bachelor of Science in Computer Engineering" },
                    { 19, 4, "Bachelor of Science in Electrical Engineering" },
                    { 20, 4, "Bachelor of Science in Electronics Engineering" },
                    { 21, 4, "Bachelor of Science in Industrial Engineering" },
                    { 22, 5, "Law" },
                    { 23, 6, "AB Political Science" },
                    { 24, 6, "BA Communication" },
                    { 25, 6, "Bachelor of Public Administration" },
                    { 26, 6, "Bachelor of Public Administration (SPECIAL PROGRAM)" },
                    { 27, 6, "Bachelor of Science in Computer Science" },
                    { 28, 6, "Bachelor of Science in Entertainment and Multimedia Computing" },
                    { 29, 6, "Bachelor of Science in Information System" },
                    { 30, 6, "Bachelor of Science in Information Technology" },
                    { 31, 6, "Bachelor of Science in Mathematics" },
                    { 32, 6, "Bachelor of Science in Psychology" },
                    { 33, 7, "Doctor in Public Administration" },
                    { 34, 7, "Doctor of Philosophy, Major in Educational Management" },
                    { 35, 7, "Master in Public Administration" },
                    { 36, 7, "Master of Arts in Education, Major in Educational Management" },
                    { 37, 7, "Master of Arts in Education, Major in Teaching in the Early Grades" },
                    { 38, 7, "Master of Arts in Education, Major in Teaching Science" },
                    { 39, 7, "Master of Business Administration" },
                    { 40, 7, "Master of Science in Criminal Justice, Major in Criminology" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Programs_CollegeId",
                table: "Programs",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProgramID",
                table: "Users",
                column: "ProgramID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_YearLevelId",
                table: "Users",
                column: "YearLevelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Programs");

            migrationBuilder.DropTable(
                name: "YearLevels");

            migrationBuilder.DropTable(
                name: "Colleges");
        }
    }
}

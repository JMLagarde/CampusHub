using CampusHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CampusHub.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static void SeedData(ModelBuilder modelBuilder)
        {
            SeedYearLevels(modelBuilder);
            SeedColleges(modelBuilder);
            SeedPrograms(modelBuilder);
            SeedAdminUser(modelBuilder);
        }

        private static void SeedYearLevels(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<YearLevel>().HasData(
                new YearLevel { Id = 1, Name = "1st Year" },
                new YearLevel { Id = 2, Name = "2nd Year" },
                new YearLevel { Id = 3, Name = "3rd Year" },
                new YearLevel { Id = 4, Name = "4th Year" },
                new YearLevel { Id = 5, Name = "5th Year" },
                new YearLevel { Id = 6, Name = "Graduate" }
            );
        }

        private static void SeedColleges(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<College>().HasData(
                new College { CollegeId = 1, CollegeName = "College of Business and Accountancy" },
                new College { CollegeId = 2, CollegeName = "College of Criminal Justice Education" },
                new College { CollegeId = 3, CollegeName = "College of Education" },
                new College { CollegeId = 4, CollegeName = "College of Engineering" },
                new College { CollegeId = 5, CollegeName = "College of Law" },
                new College { CollegeId = 6, CollegeName = "College of Liberal Arts and Sciences" },
                new College { CollegeId = 7, CollegeName = "Graduate School" }
            );
        }

        private static void SeedPrograms(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProgramEntity>().HasData(
                // Business and Accountancy
                new ProgramEntity { Id = 1, Name = "Bachelor of Science in Accountancy", CollegeId = 1 },
                new ProgramEntity { Id = 2, Name = "Bachelor of Science in Accounting Information System", CollegeId = 1 },
                new ProgramEntity { Id = 3, Name = "Bachelor of Science in Business Administration, Major in Financial Management", CollegeId = 1 },
                new ProgramEntity { Id = 4, Name = "Bachelor of Science in Business Administration, Major in Human Resource Management", CollegeId = 1 },
                new ProgramEntity { Id = 5, Name = "Bachelor of Science in Business Administration, Major in Marketing Management", CollegeId = 1 },
                new ProgramEntity { Id = 6, Name = "Bachelor of Science in Entrepreneurship", CollegeId = 1 },
                new ProgramEntity { Id = 7, Name = "Bachelor of Science in Hospitality Management", CollegeId = 1 },
                new ProgramEntity { Id = 8, Name = "Bachelor of Science in Office Administration", CollegeId = 1 },
                new ProgramEntity { Id = 9, Name = "Bachelor of Science in Tourism Management", CollegeId = 1 },

                // Criminal Justice Education
                new ProgramEntity { Id = 10, Name = "Bachelor of Science in Criminology", CollegeId = 2 },

                // Education
                new ProgramEntity { Id = 11, Name = "Bachelor in Secondary Education Major in English", CollegeId = 3 },
                new ProgramEntity { Id = 12, Name = "Bachelor in Secondary Education Major in English - Chinese", CollegeId = 3 },
                new ProgramEntity { Id = 13, Name = "Bachelor in Secondary Education Major in Science", CollegeId = 3 },
                new ProgramEntity { Id = 14, Name = "Bachelor in Secondary Education Major in Technology and Livelihood Education", CollegeId = 3 },
                new ProgramEntity { Id = 15, Name = "Bachelor of Early Childhood Education", CollegeId = 3 },
                new ProgramEntity { Id = 16, Name = "Certificate in Professional Education", CollegeId = 3 },
                new ProgramEntity { Id = 17, Name = "Elementary | Secondary | P.E.", CollegeId = 3 },

                // Engineering
                new ProgramEntity { Id = 18, Name = "Bachelor of Science in Computer Engineering", CollegeId = 4 },
                new ProgramEntity { Id = 19, Name = "Bachelor of Science in Electrical Engineering", CollegeId = 4 },
                new ProgramEntity { Id = 20, Name = "Bachelor of Science in Electronics Engineering", CollegeId = 4 },
                new ProgramEntity { Id = 21, Name = "Bachelor of Science in Industrial Engineering", CollegeId = 4 },

                // Law
                new ProgramEntity { Id = 22, Name = "Law", CollegeId = 5 },

                // Liberal Arts and Sciences
                new ProgramEntity { Id = 23, Name = "AB Political Science", CollegeId = 6 },
                new ProgramEntity { Id = 24, Name = "BA Communication", CollegeId = 6 },
                new ProgramEntity { Id = 25, Name = "Bachelor of Public Administration", CollegeId = 6 },
                new ProgramEntity { Id = 26, Name = "Bachelor of Public Administration (SPECIAL PROGRAM)", CollegeId = 6 },
                new ProgramEntity { Id = 27, Name = "Bachelor of Science in Computer Science", CollegeId = 6 },
                new ProgramEntity { Id = 28, Name = "Bachelor of Science in Entertainment and Multimedia Computing", CollegeId = 6 },
                new ProgramEntity { Id = 29, Name = "Bachelor of Science in Information System", CollegeId = 6 },
                new ProgramEntity { Id = 30, Name = "Bachelor of Science in Information Technology", CollegeId = 6 },
                new ProgramEntity { Id = 31, Name = "Bachelor of Science in Mathematics", CollegeId = 6 },
                new ProgramEntity { Id = 32, Name = "Bachelor of Science in Psychology", CollegeId = 6 },

                // Graduate School
                new ProgramEntity { Id = 33, Name = "Doctor in Public Administration", CollegeId = 7 },
                new ProgramEntity { Id = 34, Name = "Doctor of Philosophy, Major in Educational Management", CollegeId = 7 },
                new ProgramEntity { Id = 35, Name = "Master in Public Administration", CollegeId = 7 },
                new ProgramEntity { Id = 36, Name = "Master of Arts in Education, Major in Educational Management", CollegeId = 7 },
                new ProgramEntity { Id = 37, Name = "Master of Arts in Education, Major in Teaching in the Early Grades", CollegeId = 7 },
                new ProgramEntity { Id = 38, Name = "Master of Arts in Education, Major in Teaching Science", CollegeId = 7 },
                new ProgramEntity { Id = 39, Name = "Master of Business Administration", CollegeId = 7 },
                new ProgramEntity { Id = 40, Name = "Master of Science in Criminal Justice, Major in Criminology", CollegeId = 7 }
            );
        }

        private static void SeedAdminUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserID = 999,
                    Username = "admin",
                    FullName = "System Administrator",
                    Email = "admin@ucc.edu.ph",
                    Password = "admin123",
                    Role = "Admin",
                    DateRegistered = new DateTime(2025, 9, 28, 13, 50, 0, DateTimeKind.Utc),
                    StudentNumber = null,
                    ContactNumber = null,
                    YearLevelId = null,
                    ProgramID = null,
                    UpdatedAt = null,
                    ProfilePictureUrl = null
                }
            );
        }
    }
}
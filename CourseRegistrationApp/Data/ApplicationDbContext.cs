using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CourseRegistrationApp.Models;
using System;

namespace CourseRegistrationApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed Roles (Only two unique roles to avoid unique key constraint violation on NormalizedName)
            var adminRoleId = "r1";
            var studentRoleId = "r2";

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = studentRoleId, Name = "Student", NormalizedName = "STUDENT" }
            );

            // Seed Admin User
            var adminUserId = "u1";
            var adminUser = new IdentityUser
            {
                Id = adminUserId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@courses.com",
                NormalizedEmail = "ADMIN@COURSES.COM",
                EmailConfirmed = true,
                SecurityStamp = "f667954b-d77c-473d-8ca7-eb2c1e82ef45",
                ConcurrencyStamp = "5a6da1a5-8309-4e2b-9771-cb35b07ec8b8",
                PasswordHash = "AQAAAAIAAYagAAAAEEEHiJvXH6826dWHDqKNfIB+gz4SR5LNtzEtnvo0VvcAnjtiz1Dl/Yd1gcs/HyPfJA=="
            };

            builder.Entity<IdentityUser>().HasData(adminUser);

            // Assign Admin Role to Admin User
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = adminUserId, RoleId = adminRoleId }
            );

            // Seed Categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Công nghệ thông tin" },
                new Category { Id = 2, Name = "Kinh tế & Kinh doanh" },
                new Category { Id = 3, Name = "Ngoại ngữ" }
            );

            // Seed Courses
            builder.Entity<Course>().HasData(
                new Course { Id = 1, Name = "Lập trình ASP.NET Core MVC", Credits = 3, Lecturer = "Nguyễn Văn A", CategoryId = 1, Image = "aspnetcore.jpg" },
                new Course { Id = 2, Name = "Cơ sở dữ liệu SQL Server", Credits = 3, Lecturer = "Trần Thị B", CategoryId = 1, Image = "sqlserver.jpg" },
                new Course { Id = 3, Name = "Kinh tế vĩ mô", Credits = 2, Lecturer = "Phạm Văn C", CategoryId = 2, Image = "macroeconomics.jpg" },
                new Course { Id = 4, Name = "Tiếng Anh chuyên ngành IT", Credits = 2, Lecturer = "Lê Thị D", CategoryId = 3, Image = "english.jpg" },
                new Course { Id = 5, Name = "Phát triển ứng dụng di động", Credits = 4, Lecturer = "Nguyễn Văn A", CategoryId = 1, Image = "mobiledev.jpg" },
                new Course { Id = 6, Name = "Quản trị học đại cương", Credits = 3, Lecturer = "Đỗ Hoàng E", CategoryId = 2, Image = "management.jpg" }
            );
        }
    }
}

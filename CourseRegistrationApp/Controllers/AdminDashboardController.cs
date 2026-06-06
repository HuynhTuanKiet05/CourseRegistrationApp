using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseRegistrationApp.Data;
using CourseRegistrationApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseRegistrationApp.Controllers
{
    [Authorize(Roles = "Admin,ADMIN")]
    [Route("admin/dashboard")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminDashboardController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        // GET: /admin/dashboard
        [Route("")]
        public async Task<IActionResult> Index()
        {
            // Total Courses
            int totalCourses = await _context.Courses.CountAsync();

            // Total Students (Count users in Student role)
            int totalStudents = 0;
            var studentRole = await _roleManager.FindByNameAsync("Student");
            if (studentRole != null)
            {
                totalStudents = await _context.UserRoles
                    .Where(ur => ur.RoleId == studentRole.Id)
                    .CountAsync();
            }
            else
            {
                // Fallback to STUDENT role
                var studentRoleUpper = await _roleManager.FindByNameAsync("STUDENT");
                if (studentRoleUpper != null)
                {
                    totalStudents = await _context.UserRoles
                        .Where(ur => ur.RoleId == studentRoleUpper.Id)
                        .CountAsync();
                }
            }

            // Total Enrollments
            int totalEnrollments = await _context.Enrollments.CountAsync();

            // Statistics of students enrolled per course
            var courseStats = await _context.Courses
                .Select(c => new CourseStatItem
                {
                    CourseName = c.Name,
                    StudentCount = c.Enrollments.Count
                })
                .ToListAsync();

            var viewModel = new AdminDashboardViewModel
            {
                TotalCourses = totalCourses,
                TotalStudents = totalStudents,
                TotalEnrollments = totalEnrollments,
                CourseStats = courseStats
            };

            return View(viewModel);
        }
    }
}

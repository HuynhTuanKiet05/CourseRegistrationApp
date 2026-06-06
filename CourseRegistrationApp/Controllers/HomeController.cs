using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CourseRegistrationApp.Data;
using CourseRegistrationApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseRegistrationApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Mapping multiple routes as required by the test guidelines
        [Route("")]
        [Route("home")]
        [Route("courses")]
        public async Task<IActionResult> Index(string? searchTerm, int page = 1)
        {
            if (page < 1) page = 1;
            int pageSize = 5;

            IQueryable<Course> query = _context.Courses.Include(c => c.Category);

            // Câu 8: Tìm kiếm học phần theo tên
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.Name.Contains(searchTerm));
            }

            int totalCourses = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalCourses / pageSize);
            if (totalPages == 0) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var courses = await query
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Fetch enrolled courses for the logged-in student (Câu 6)
            var enrolledCourseIds = new List<int>();
            if (User.Identity?.IsAuthenticated == true && (User.IsInRole("Student") || User.IsInRole("STUDENT")))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null)
                {
                    enrolledCourseIds = await _context.Enrollments
                        .Where(e => e.UserId == userId)
                        .Select(e => e.CourseId)
                        .ToListAsync();
                }
            }

            var viewModel = new HomeViewModel
            {
                Courses = courses,
                SearchTerm = searchTerm ?? string.Empty,
                CurrentPage = page,
                TotalPages = totalPages,
                EnrolledCourseIds = enrolledCourseIds
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

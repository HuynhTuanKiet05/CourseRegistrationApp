using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CourseRegistrationApp.Data;
using CourseRegistrationApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseRegistrationApp.Controllers
{
    [Authorize(Roles = "Student,STUDENT")]
    [Route("enroll")]
    public class EnrollController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnrollController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: /enroll/add
        [HttpPost]
        [Route("add")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Challenge();
            }

            // Check if course exists
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound("Học phần không tồn tại.");
            }

            // Check if already enrolled
            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (!alreadyEnrolled)
            {
                var enrollment = new Enrollment
                {
                    UserId = userId,
                    CourseId = courseId,
                    EnrollDate = DateTime.Now
                };
                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();
            }

            // Redirect back to referring page or Home
            var referrer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referrer))
            {
                return Redirect(referrer);
            }
            return RedirectToAction("Index", "Home");
        }

        // POST: /enroll/remove
        [HttpPost]
        [Route("remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Challenge();
            }

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }

            // Redirect back to referring page or My Courses
            var referrer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referrer))
            {
                return Redirect(referrer);
            }
            return RedirectToAction("MyCourses");
        }

        // GET: /enroll/my-courses
        [HttpGet]
        [Route("my-courses")]
        public async Task<IActionResult> MyCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Challenge();
            }

            var enrolledCourses = await _context.Enrollments
                .Where(e => e.UserId == userId)
                .Include(e => e.Course)
                .ThenInclude(c => c!.Category)
                .Select(e => e.Course)
                .ToListAsync();

            return View(enrolledCourses);
        }
    }
}

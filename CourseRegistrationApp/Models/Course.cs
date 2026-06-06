using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseRegistrationApp.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên học phần là bắt buộc.")]
        [StringLength(200, ErrorMessage = "Tên học phần không được vượt quá 200 ký tự.")]
        public string Name { get; set; } = string.Empty;

        public string? Image { get; set; }

        [Required(ErrorMessage = "Số tín chỉ là bắt buộc.")]
        [Range(1, 10, ErrorMessage = "Số tín chỉ phải từ 1 đến 10.")]
        public int Credits { get; set; }

        [Required(ErrorMessage = "Tên giảng viên là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên giảng viên không được vượt quá 100 ký tự.")]
        public string Lecturer { get; set; } = string.Empty;

        [Required(ErrorMessage = "Danh mục là bắt buộc.")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        // Navigation property
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}

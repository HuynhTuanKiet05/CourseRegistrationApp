using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseRegistrationApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = string.Empty;

        // Navigation property
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}

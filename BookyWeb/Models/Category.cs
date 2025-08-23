using System.ComponentModel.DataAnnotations;

namespace BookyWeb.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [MinLength(3, ErrorMessage ="Category name must be at least 3 characters")]
        [MaxLength(100, ErrorMessage ="Category name cannot exceed 100 characters")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Only letters are allowed.")]
        public string Name { get; set; }

        [Display(Name = "Display Order")]
        [Range(1,100, ErrorMessage = "Display Order must be in range of 1 - 100")]
        public int DisplayOrder { get; set; }
    }
}

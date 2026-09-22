using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Shared.DTOs.Categories
{
    public class SaveCategoryDto
    {
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(50, ErrorMessage = "Category name must be 50 characters or less.")]
        public string? Name { get; set; }
        public string Color { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}

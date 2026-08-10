using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceTracker.Shared.DTOs.Categories
{
    public class SaveCategoryDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public string? Color { get; set; } = "#3B5BDB";

        public string? Icon { get; set; } = "📁";
    }
}

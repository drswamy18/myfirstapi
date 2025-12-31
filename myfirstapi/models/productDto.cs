using System.ComponentModel.DataAnnotations;

namespace ProductApi.Models
{
    // public class ProductDto
    // {
    //     [Required]
    //     [StringLength(100, MinimumLength = 3)]
    //     public string Name { get; set; } = string.Empty;

    //     [StringLength(500)]
    //     public string Description { get; set; } = string.Empty;

    //     [Range(0.01, 10000)]
    //     public decimal Price { get; set; }

    //     [Range(0, 10000)]
    //     public int Stock { get; set; }
    // }
    public class ProductDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 10000)]
        public decimal Price { get; set; }
        [Range(0, 10000)]
        public int Stock { get; set; }
    }
}
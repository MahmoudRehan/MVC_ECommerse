using System.ComponentModel.DataAnnotations;

namespace PP.ViewModels
{
    public class ProductVM
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "SKU is required")]
        [StringLength(50)]
        public string SKU { get; set; }

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 100000)]
        public int StockQuantity { get; set; }

      
        public IFormFile? Image { get; set; }

        public string? ExistingImage { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public bool IsActive { get; set; }
    }
}

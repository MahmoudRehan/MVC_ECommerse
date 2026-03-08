using System.ComponentModel.DataAnnotations;

namespace PP.ViewModels
{
 
    public class CheckoutVM
    {
        
        [Required(ErrorMessage = "Street address is required")]
        [StringLength(200)]
        [Display(Name = "Street Address")]
        public string Street { get; set; }

       
        [Required(ErrorMessage = "City is required")]
        [StringLength(100)]
        [Display(Name = "City")]
        public string City { get; set; }

        
        [Required(ErrorMessage = "Zip code is required")]
        [StringLength(20)]
        [Display(Name = "Zip Code")]
        public string Zip { get; set; }

       
        [Required(ErrorMessage = "Country is required")]
        [StringLength(100)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

       
        public decimal TotalAmount { get; set; }
    }
}


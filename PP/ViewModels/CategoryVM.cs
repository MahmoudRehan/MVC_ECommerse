using System.ComponentModel.DataAnnotations;

namespace PP.ViewModels
{
    public class CategoryVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100)]
        public string Name { get; set; }
        
    }
}

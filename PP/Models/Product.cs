using System.ComponentModel.DataAnnotations;

namespace PP.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string SKU { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string PictureUrl { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime  CreatedAt { get; set; }= DateTime.UtcNow;
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}

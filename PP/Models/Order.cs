namespace PP.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string OrderNumber { get; set; }

        public int Status { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public decimal TotalAmount { get; set; }

        public int ShippingAddressId { get; set; }

        public Address ShippingAddress { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}

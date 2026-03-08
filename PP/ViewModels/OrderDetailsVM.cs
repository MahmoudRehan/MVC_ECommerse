using PP.Models;

namespace PP.ViewModels
{
    
    public class OrderDetailsVM
    {
       public int Id { get; set; }
        public string OrderNumber { get; set; }

        
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Order status
        /// 0 = Pending, 1 = Processing, 2 = Shipped, 3 = Delivered, 4 = Cancelled
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Total amount of the order
        /// Sum of all OrderItems LineTotal
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Shipping address for this order
        /// </summary>
        public Address ShippingAddress { get; set; }

        /// <summary>
        /// List of items in this order
        /// </summary>
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        /// <summary>
        /// Gets the status as a readable string
        /// </summary>
        public string StatusText => Status switch
        {
            0 => "Pending",
            1 => "Processing",
            2 => "Shipped",
            3 => "Delivered",
            4 => "Cancelled",
            
        };

        /// <summary>
        /// Gets the Bootstrap badge color for status
        /// </summary>
        public string StatusBadgeClass => Status switch
        {
            0 => "bg-warning text-dark",
            1 => "bg-info",
            2 => "bg-primary",
            3 => "bg-success",
            4 => "bg-danger",
         
        };
    }
}

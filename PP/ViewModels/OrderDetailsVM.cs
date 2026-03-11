using PP.Models;

namespace PP.ViewModels
{
    
    public class OrderDetailsVM
    {
       public int Id { get; set; }
        public string OrderNumber { get; set; }

        
        public DateTime OrderDate { get; set; }

        
        public int Status { get; set; }

       
        public decimal TotalAmount { get; set; }

        
        public Address ShippingAddress { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        
        public string StatusText => Status switch
        {
            0 => "Pending",
            1 => "Processing",
            2 => "Shipped",
            3 => "Delivered",
            4 => "Cancelled",
            
        };

       
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

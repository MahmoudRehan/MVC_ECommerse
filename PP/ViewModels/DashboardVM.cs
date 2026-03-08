using PP.Models;

namespace PP.ViewModels
{
    public class DashboardVM
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }

        public decimal Revenue { get; set; }

        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}

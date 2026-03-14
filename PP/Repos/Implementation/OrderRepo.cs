using Microsoft.EntityFrameworkCore;
using PP.Models;
using PP.Models.Context;
using PP.Repos.Inerfaces;

namespace PP.Repos.Implementation
{
    
    public class OrderRepo : GenericRepo<Order>, IOrderRepo
    {
        private readonly AppDbContext _context;

        public OrderRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }

        
        public Order GetOrderWithDetails(int orderId)
        {
            return _context.Orders
                .Include(o => o.OrderItems)          // Include order items
                    .ThenInclude(oi => oi.Product)   // Include product details for each item
                .Include(o => o.ShippingAddress)     // Include shipping address
                .Include(o => o.User)                // Include the customer who placed the order
                .FirstOrDefault(o => o.OrderId == orderId);
        }

       
        public Order GetOrderByNumber(string orderNumber)
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingAddress)
                .FirstOrDefault(o => o.OrderNumber == orderNumber);
        }

       
        public IEnumerable<Order> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.ShippingAddress)
                .Include(o => o.OrderItems)
                .Include(o => o.User)                // Include the customer for email display
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public IEnumerable<Order> GetAllCompletedOrders()
        {
            return _context.Orders
                .Include(o => o.ShippingAddress)
                .Include(o => o.OrderItems)
                .Where(o => o.Status == 3) 
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }
        public IEnumerable<Order> GetAllPendingOrders()
        {
            return _context.Orders
                .Include(o => o.ShippingAddress)
                .Include(o => o.OrderItems)
                .Where(o => o.Status == 0) 
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public IEnumerable<Order> GetUserOrder(string userId)
        {
            return _context.Orders
                .Include(o => o.ShippingAddress)
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PP.Models;
using PP.Models.Context;
using PP.Repos.Inerfaces;

namespace PP.Repos.Implementation
{
    /// <summary>
    /// Repository for Order entity operations
    /// Handles database operations for orders including related entities
    /// </summary>
    public class OrderRepo : GenericRepo<Order>, IOrderRepo
    {
        private readonly AppDbContext _context;

        public OrderRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Get order with all related data (OrderItems, Products, ShippingAddress)
        /// Used for displaying complete order details
        /// </summary>
        public Order GetOrderWithDetails(int orderId)
        {
            return _context.Orders
                .Include(o => o.OrderItems)          // Include order items
                    .ThenInclude(oi => oi.Product)   // Include product details for each item
                .Include(o => o.ShippingAddress)     // Include shipping address
                .FirstOrDefault(o => o.OrderId == orderId);
        }

        /// <summary>
        /// Get order by its unique order number
        /// </summary>
        public Order GetOrderByNumber(string orderNumber)
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingAddress)
                .FirstOrDefault(o => o.OrderNumber == orderNumber);
        }

        /// <summary>
        /// Get all orders with related data
        /// Used for order listing pages
        /// </summary>
        public IEnumerable<Order> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.ShippingAddress)
                .Include(o => o.OrderItems)
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
    }
}

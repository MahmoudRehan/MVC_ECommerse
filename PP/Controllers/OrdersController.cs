using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PP.Models;
using PP.Models.Context;
using PP.Repos.Inerfaces;
using PP.Services;
using PP.ViewModels;

namespace PP.Controllers
{
    /// <summary>
    /// Controller for handling order placement and checkout operations
    /// Converts session-based cart into database-persisted orders
    /// </summary>
    public class OrdersController : Controller
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IAddressRepo _addressRepo;
        private readonly IProductRepo _productRepo;
        private readonly AppDbContext _context;

        public OrdersController(
            IOrderRepo orderRepo, 
            IAddressRepo addressRepo, 
            IProductRepo productRepo,
            AppDbContext context)
        {
            _orderRepo = orderRepo;
            _addressRepo = addressRepo;
            _productRepo = productRepo;
            _context = context;
        }

       
        [HttpGet]
        public IActionResult Checkout()
        {
            // Step 1: Retrieve cart from session storage
            // CartService deserializes the JSON stored in session into List<CartItem>
            var cart = CartService.GetCart(HttpContext.Session);

            // Step 2: Validate cart is not empty
            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Your cart is empty. Please add items before checkout.";
                return RedirectToAction("Index", "Cart");
            }

            // Step 3: Prepare checkout view model
            var checkoutVM = new CheckoutVM
            {
                CartItems = cart,
                TotalAmount = CartService.GetCartTotal(cart)
            };

            return View(checkoutVM);
        }

        /// <summary>
        /// POST: Process checkout and create order in database
        /// This method converts session cart into a permanent database order
        /// Uses transaction to ensure data consistency
        /// Creates new address from user input during checkout
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(CheckoutVM checkoutVM)
        {
            // Step 1: Retrieve cart from session
            var cart = CartService.GetCart(HttpContext.Session);

            // Step 2: Validate cart exists and has items
            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            // Step 3: Validate model state (all address fields are required)
            if (!ModelState.IsValid)
            {
                checkoutVM.CartItems = cart;
                checkoutVM.TotalAmount = CartService.GetCartTotal(cart);
                return View(checkoutVM);
            }

            

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    
                    var shippingAddress = new Address
                    {
                        Street = checkoutVM.Street,
                        City = checkoutVM.City,
                        Zip = checkoutVM.Zip,
                        Country = checkoutVM.Country,
                        IsDefault = false 
                    };

                    _addressRepo.Add(shippingAddress);
                    _addressRepo.Save();

                
                    //  CREATE ORDER
                    
                    // Generate unique order number using timestamp + random number
                  
                    var orderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";

                    var order = new Order
                    {
                        OrderNumber = orderNumber,
                        OrderDate = DateTime.UtcNow,
                        Status = 0, // 0 = Pending
                        TotalAmount = CartService.GetCartTotal(cart),
                        ShippingAddressId = shippingAddress.Id // Link to newly created address
                    };

       
                    _orderRepo.Add(order);
                    _orderRepo.Save();

                    
                    // CONVERT CART ITEMS TO ORDER ITEMS
                    

                    foreach (var cartItem in cart)
                    {
                        // Get current product from database to check stock
                        var product = _productRepo.GetById(cartItem.ProductId);

                        if (product == null)
                        {
                            throw new Exception($"Product {cartItem.ProductName} not found.");
                        }

                        
                     
                        if (product.StockQuantity < cartItem.Quantity)
                        {
                            throw new Exception($"Not enough stock for {product.Name}. Available: {product.StockQuantity}, Requested: {cartItem.Quantity}");
                        }

                        // Create OrderItem from CartItem
                        var orderItem = new OrderItem
                        {
                            OrderId = order.OrderId,
                            ProductId = cartItem.ProductId,
                            UnitPrice = cartItem.Price,      
                            Quantity = cartItem.Quantity,
                            LineTotal = cartItem.TotalPrice   
                        };

                       
                        _context.OrderItems.Add(orderItem);

                       
                        product.StockQuantity -= cartItem.Quantity;
                        _productRepo.Update(product);
                    }

                    // Step 7: Save all changes to database
                    _orderRepo.Save();

                   
                    transaction.Commit();

                   
                    CartService.ClearCart(HttpContext.Session);

                    // Step 10: Show success message and redirect to order confirmation
                    TempData["Success"] = "Order placed successfully!";
                    return RedirectToAction("Confirmation", new { orderNumber = order.OrderNumber });
                }
                catch (Exception ex)
                {
                 
                    transaction.Rollback();

                    TempData["Error"] = $"Failed to place order: {ex.Message}";
                    
                  
                    checkoutVM.CartItems = cart;
                    checkoutVM.TotalAmount = CartService.GetCartTotal(cart);
                    return View(checkoutVM);
                }
            }
        }

        
        public IActionResult Confirmation(string orderNumber)
        {
            // Get order from database with all details
            var order = _orderRepo.GetOrderByNumber(orderNumber);

            if (order == null)
            {
                return NotFound();
            }

            // Map Order entity to OrderDetailsVM for display
            var orderDetailsVM = new OrderDetailsVM
            {
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                OrderItems = order.OrderItems.ToList()
            };

            return View(orderDetailsVM);
        }

       
        public IActionResult Details(int id)
        {
            var order = _orderRepo.GetOrderWithDetails(id);

            if (order == null)
            {
                return NotFound();
            }

            var orderDetailsVM = new OrderDetailsVM
            {
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                OrderItems = order.OrderItems.ToList()
            };

            return View(orderDetailsVM);
        }

        /// <summary>
        /// Display list of all orders
        /// </summary>
        public IActionResult Index()
        {
            var orders = _orderRepo.GetAllOrders();
            return View(orders);
        }
    }
}

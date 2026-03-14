using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PP.Models;
using PP.Models.Context;
using PP.Repos.Inerfaces;
using PP.Services;
using PP.ViewModels;
using System.Security.Claims;

namespace PP.Controllers
{
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

       
        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = CartService.GetCart(HttpContext.Session);

            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Your cart is empty. Please add items before checkout.";
                return RedirectToAction("Index", "Cart");
            }

            var checkoutVM = new CheckoutVM
            {
                CartItems = cart,
                TotalAmount = CartService.GetCartTotal(cart)
            };

            return View(checkoutVM);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(CheckoutVM checkoutVM)
        {
            var cart = CartService.GetCart(HttpContext.Session);

            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            if (!ModelState.IsValid)
            {
                checkoutVM.CartItems = cart;
                checkoutVM.TotalAmount = CartService.GetCartTotal(cart);
                return View(checkoutVM);
            }

            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
                        IsDefault = false,
                        UserId = userId 
                    };

                    _addressRepo.Add(shippingAddress);
                    _addressRepo.Save();

                    var orderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";

                    var order = new Order
                    {
                        OrderNumber = orderNumber,
                        OrderDate = DateTime.UtcNow,
                        Status = 0,
                        TotalAmount = CartService.GetCartTotal(cart),
                        ShippingAddressId = shippingAddress.Id,
                        UserId = userId 
                    };

                    _orderRepo.Add(order);
                    _orderRepo.Save();

                    foreach (var cartItem in cart)
                    {
                        var product = _productRepo.GetById(cartItem.ProductId);

                        if (product == null)
                            throw new Exception($"Product {cartItem.ProductName} not found.");

                        if (product.StockQuantity < cartItem.Quantity)
                            throw new Exception($"Not enough stock for {product.Name}. Available: {product.StockQuantity}, Requested: {cartItem.Quantity}");

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

                    _orderRepo.Save();
                    transaction.Commit();

                    CartService.ClearCart(HttpContext.Session);

                    TempData["Success"] = "Order placed successfully!";
                    return RedirectToAction("Confirmation", new { OrderId = order.OrderId });
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

        public IActionResult Confirmation(int OrderId)
        {
            var order = _orderRepo.GetOrderWithDetails(OrderId);

            if (order == null)
                return NotFound();

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
                return NotFound();

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
        
        

        [Authorize]
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = _orderRepo.GetUserOrder(userId);
            return View(orders);
        }
    }
}

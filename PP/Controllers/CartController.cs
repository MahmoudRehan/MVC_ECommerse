using Microsoft.AspNetCore.Mvc;
using PP.Repos.Inerfaces;
using PP.Services;
using PP.ViewModels;

namespace PP.Controllers
{
   
    public class CartController : Controller
    {
        private readonly IProductRepo _productRepo;

        public CartController(IProductRepo productRepo)
        {
            _productRepo = productRepo;
        }

     
        public IActionResult Index()
        {
            var cart = CartService.GetCart(HttpContext.Session, User);

            ViewBag.CartTotal = CartService.GetCartTotal(cart);
            ViewBag.ItemCount = CartService.GetCartItemCount(cart);

            return View(cart);
        }

       
        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            // Get the active product from database
            var product = _productRepo.GetActiveProductById(productId);

            // Validate product exists and is active
            if (product == null)
            {
                TempData["Error"] = "Product not found or unavailable.";
                return RedirectToAction("Index", "Catalog");
            }

            // Check if product is in stock
            if (product.StockQuantity <= 0)
            {
                TempData["Error"] = "Product is out of stock.";
                return RedirectToAction("Details", "Catalog", new { id = productId });
            }

            // Retrieve current cart from session
            var cart = CartService.GetCart(HttpContext.Session, User);

            // Check if product already exists in cart
            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (existingItem != null)
            {
                // if product already in cart will increase quantity
                // Check if we have enough stock
                if (existingItem.Quantity + 1 > product.StockQuantity)
                {
                    TempData["Error"] = "Cannot add more. Not enough stock available.";
                    return RedirectToAction("Index");
                }

                existingItem.Quantity++;
            }
            else
            {
                // if product not in cart will add new cart item
                var cartItem = new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1,
                    PictureUrl = product.PictureUrl
                };

                cart.Add(cartItem);
            }

            // Save updated cart back to session
            CartService.SaveCart(HttpContext.Session, cart, User);

            TempData["Success"] = $"{product.Name} added to cart!";

            return RedirectToAction("Index");
        }

        
        [HttpPost]
        public IActionResult Remove(int productId)
        {
            var cart = CartService.GetCart(HttpContext.Session, User);

            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                cart.Remove(item);
                TempData["Success"] = "Item removed from cart.";
            }

            CartService.SaveCart(HttpContext.Session, cart, User);

            return RedirectToAction("Index");
        }

      
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                TempData["Error"] = "Quantity must be at least 1.";
                return RedirectToAction("Index");
            }

            var product = _productRepo.GetActiveProductById(productId);

            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Index");
            }

            if (quantity > product.StockQuantity)
            {
                TempData["Error"] = $"Only {product.StockQuantity} units available in stock.";
                return RedirectToAction("Index");
            }

            var cart = CartService.GetCart(HttpContext.Session, User);

            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                item.Quantity = quantity;
                TempData["Success"] = "Cart updated successfully.";
            }

            CartService.SaveCart(HttpContext.Session, cart, User);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            CartService.ClearCart(HttpContext.Session, User);

            TempData["Success"] = "Cart cleared successfully.";

            return RedirectToAction("Index");
        }
    }
}

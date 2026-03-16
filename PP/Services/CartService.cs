using PP.ViewModels;
using System.Security.Claims;
using System.Text.Json;

namespace PP.Services
{
  
    public static class CartService
    {
        private const string GuestCartSessionKey = "ShoppingCart_Guest";

        private static string GetUserCartSessionKey(string userId)
        {
            return $"ShoppingCart_{userId}";
        }

        private static string GetCartSessionKey(ClaimsPrincipal? user)
        {
            if (user?.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    return GetUserCartSessionKey(userId);
                }
            }

            return GuestCartSessionKey;
        }

       
        public static List<CartItem> GetCart(ISession session)
        {
            return GetCart(session, null);
        }

        public static List<CartItem> GetCart(ISession session, ClaimsPrincipal? user)
        {
            var cartJson = session.GetString(GetCartSessionKey(user));

            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }

            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

       

        public static void SaveCart(ISession session, List<CartItem> cart)
        {
            SaveCart(session, cart, null);
        }

        public static void SaveCart(ISession session, List<CartItem> cart, ClaimsPrincipal? user)
        {
            var cartJson = JsonSerializer.Serialize(cart);

            session.SetString(GetCartSessionKey(user), cartJson);
        }

       
        public static void ClearCart(ISession session)
        {
            ClearCart(session, null);
        }

        public static void ClearCart(ISession session, ClaimsPrincipal? user)
        {
            session.Remove(GetCartSessionKey(user));
        }

        
        public static decimal GetCartTotal(List<CartItem> cart)
        {
            return cart.Sum(item => item.TotalPrice);
        }

       
        public static int GetCartItemCount(List<CartItem> cart)
        {
            return cart.Sum(item => item.Quantity);
        }

        public static void MergeGuestCartIntoUserCart(ISession session, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return;

            var guestCartJson = session.GetString(GuestCartSessionKey);
            var userCartKey = GetUserCartSessionKey(userId);
            var userCartJson = session.GetString(userCartKey);

            var guestCart = string.IsNullOrEmpty(guestCartJson)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(guestCartJson) ?? new List<CartItem>();

            if (!guestCart.Any())
                return;

            var userCart = string.IsNullOrEmpty(userCartJson)
                ? new List<CartItem>()
                : JsonSerializer.Deserialize<List<CartItem>>(userCartJson) ?? new List<CartItem>();

            foreach (var guestItem in guestCart)
            {
                var existingItem = userCart.FirstOrDefault(i => i.ProductId == guestItem.ProductId);

                if (existingItem == null)
                {
                    userCart.Add(guestItem);
                }
                else
                {
                    existingItem.Quantity += guestItem.Quantity;
                }
            }

            session.SetString(userCartKey, JsonSerializer.Serialize(userCart));
            session.Remove(GuestCartSessionKey);
        }
    }
}

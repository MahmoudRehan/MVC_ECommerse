using PP.ViewModels;
using System.Text.Json;

namespace PP.Services
{
    
    public static class CartService
    {
        private const string CartSessionKey = "ShoppingCart";

       
        public static List<CartItem> GetCart(ISession session)
        {
            var cartJson = session.GetString(CartSessionKey);

            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }

            // Deserialize the JSON string back into a List<CartItem>
            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

       

        public static void SaveCart(ISession session, List<CartItem> cart)
        {
            // Serialize the cart list to JSON string
            var cartJson = JsonSerializer.Serialize(cart);

            session.SetString(CartSessionKey, cartJson);
        }

       
        public static void ClearCart(ISession session)
        {
            session.Remove(CartSessionKey);
        }

        
        public static decimal GetCartTotal(List<CartItem> cart)
        {
            return cart.Sum(item => item.TotalPrice);
        }

       
        public static int GetCartItemCount(List<CartItem> cart)
        {
            return cart.Sum(item => item.Quantity);
        }
    }
}

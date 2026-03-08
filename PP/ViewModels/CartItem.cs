namespace PP.ViewModels
{
    /// <summary>
    /// Represents an item in the shopping cart
    /// This ViewModel is used to store cart data in Session
    /// </summary>
    public class CartItem
    {
        public int ProductId { get; set; }
        
        public string ProductName { get; set; }
        
        public decimal Price { get; set; }
        
        public int Quantity { get; set; }
        
        public string PictureUrl { get; set; }
        
        /// <summary>
        /// Calculated property: Price * Quantity
        /// Returns the total price for this cart item
        /// </summary>
        public decimal TotalPrice => Price * Quantity;
    }
}

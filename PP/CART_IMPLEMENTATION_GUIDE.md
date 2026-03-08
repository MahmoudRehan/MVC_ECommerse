# Shopping Cart Module - Implementation Guide

## 📋 Overview
This shopping cart uses **Session-based storage** to maintain cart data across HTTP requests without requiring database storage or user authentication.

---

## 🔧 How Session Storage Works

### What is Session?
- **Session** is a server-side storage mechanism that maintains user-specific data across multiple HTTP requests
- Each user gets a unique session identified by a session cookie
- Data is stored on the server (in memory or distributed cache)
- The session cookie is sent with each request to identify the user

### Session in This Project:
1. **Configuration** (Program.cs):
   ```csharp
   builder.Services.AddDistributedMemoryCache(); // Stores session data in server memory
   builder.Services.AddSession(options => {
       options.IdleTimeout = TimeSpan.FromMinutes(30); // Session expires after 30 min
   });
   ```

2. **Middleware** (Program.cs):
   ```csharp
   app.UseSession(); // Must be called before UseAuthorization
   ```

3. **Storage Format**:
   - Cart items are stored as a JSON string in session
   - Key: `"ShoppingCart"`
   - Value: JSON serialized `List<CartItem>`

---

## 🛒 How Items Are Added to Cart

### Step-by-Step Flow:

1. **User clicks "Add to Cart"** on product
   - Form submits POST request to `Cart/AddToCart` with `productId`

2. **Controller receives request** (CartController.AddToCart):
   ```csharp
   // Step 1: Get product from database
   var product = _productRepo.GetActiveProductById(productId);
   
   // Step 2: Validate product and stock
   if (product == null || product.StockQuantity <= 0) 
       return with error;
   
   // Step 3: Retrieve current cart from session
   var cart = CartService.GetCart(HttpContext.Session);
   
   // Step 4: Check if product already in cart
   var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);
   
   if (existingItem != null) {
       // Product exists - increment quantity
       existingItem.Quantity++;
   } else {
       // New product - create CartItem and add to cart
       cart.Add(new CartItem { ... });
   }
   
   // Step 5: Save updated cart back to session
   CartService.SaveCart(HttpContext.Session, cart);
   ```

3. **Session storage process**:
   - `CartService.SaveCart()` serializes the cart list to JSON
   - JSON is stored in session with key "ShoppingCart"
   - Session cookie tracks the user's session ID

4. **User redirected to cart page**:
   - Cart retrieved from session
   - Displayed with totals calculated

---

## 📦 Cart Data Flow

```
User Action (Add to Cart)
    ↓
CartController.AddToCart(productId)
    ↓
Get Product from Database
    ↓
Retrieve Cart from Session (JSON → List<CartItem>)
    ↓
Add/Update Product in Cart List
    ↓
Save Cart to Session (List<CartItem> → JSON)
    ↓
Redirect to Cart Index
    ↓
Display Cart (Retrieved from Session)
```

---

## 🔑 Key Components

### 1. CartItem ViewModel
- Stores: ProductId, ProductName, Price, Quantity, PictureUrl
- Calculated: TotalPrice = Price × Quantity

### 2. CartService (Helper Class)
- **GetCart()**: Retrieves cart from session (deserializes JSON)
- **SaveCart()**: Saves cart to session (serializes to JSON)
- **ClearCart()**: Removes cart from session
- **GetCartTotal()**: Calculates sum of all item totals
- **GetCartItemCount()**: Calculates total quantity across all items

### 3. CartController
- **Index**: Display cart
- **AddToCart**: Add product or increment quantity
- **Remove**: Remove item from cart
- **UpdateQuantity**: Change item quantity
- **ClearCart**: Empty entire cart

---

## 💡 Session vs Database Storage

### Why Session?
✅ No database queries needed for cart operations  
✅ Fast performance  
✅ Works without user login  
✅ Automatically expires after inactivity  
✅ Simple implementation  

### Limitations:
❌ Data lost if session expires or server restarts  
❌ Not suitable for "save for later" features  
❌ Can't track abandoned carts  

---

## 🎯 Future Enhancements
- Add cart item count badge to navigation
- Implement checkout process
- Persist cart to database on user login
- Add wishlist functionality


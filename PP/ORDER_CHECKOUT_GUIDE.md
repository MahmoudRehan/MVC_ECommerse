# Order / Checkout Module - Implementation Guide

## 📋 Overview
This module converts the **Session-based Shopping Cart** into a **Database-persisted Order**.

---

## 🔄 Checkout Flow (Step-by-Step)

### 1️⃣ User Navigates to Checkout
```
Cart Index Page → Click "Proceed to Checkout" → Orders/Checkout (GET)
```

**What Happens:**
- `OrdersController.Checkout()` GET action is called
- Cart is retrieved from Session using `CartService.GetCart()`
- If cart is empty → redirect to Cart page with error message
- If cart has items → display checkout page with:
  - Cart items summary
  - Total amount
  - Shipping address dropdown

---

### 2️⃣ User Selects Address and Clicks "Place Order"
```
Orders/Checkout Page → Select Address → Click "Place Order" → Orders/Checkout (POST)
```

**What Happens in OrdersController.Checkout() POST:**

#### **Step A: Retrieve & Validate Cart**
```csharp
var cart = CartService.GetCart(HttpContext.Session);
if (cart == null || !cart.Any()) 
    return error;
```
- Cart is retrieved from session storage
- Validates cart exists and has items

#### **Step B: Validate Address**
```csharp
var address = _addressRepo.GetById(checkoutVM.AddressId);
if (address == null) 
    return error;
```
- Verifies selected shipping address exists in database

#### **Step C: Begin Database Transaction**
```csharp
using (var transaction = _context.Database.BeginTransaction())
{
    // All database operations happen here
}
```

**WHY USE TRANSACTION?**
- We perform multiple related database operations:
  1. Create Order
  2. Create multiple OrderItems
  3. Decrease stock for multiple products
- **Atomicity**: All operations must succeed together or fail together
- **Example scenario**: If stock update fails for product #3, we don't want:
  - Order created ✅
  - OrderItems 1-2 created ✅
  - Stock decreased for products 1-2 ✅
  - Product 3 stock update FAILS ❌
- Without transaction: Partial order exists with wrong stock levels (BAD!)
- With transaction: Everything rolls back, database stays consistent (GOOD!)

#### **Step D: Create Order**
```csharp
var orderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{Random().Next(1000, 9999)}";

var order = new Order
{
    OrderNumber = orderNumber,        // Unique tracking number
    OrderDate = DateTime.UtcNow,      // Current timestamp
    Status = 0,                       // 0 = Pending
    TotalAmount = GetCartTotal(cart), // Sum of all cart items
    ShippingAddressId = AddressId     // Where to ship
};

_orderRepo.Add(order);
_orderRepo.Save(); // Must save to get OrderId for OrderItems
```

#### **Step E: Convert CartItems → OrderItems**
```csharp
foreach (var cartItem in cart)
{
    // Get fresh product data from database
    var product = _productRepo.GetById(cartItem.ProductId);
    
    // STOCK VALIDATION
    if (product.StockQuantity < cartItem.Quantity)
        throw exception; // Not enough stock
    
    // CREATE ORDER ITEM
    var orderItem = new OrderItem
    {
        OrderId = order.OrderId,
        ProductId = cartItem.ProductId,
        UnitPrice = cartItem.Price,      // Price at purchase time
        Quantity = cartItem.Quantity,
        LineTotal = cartItem.TotalPrice  // Price × Quantity
    };
    
    _context.OrderItems.Add(orderItem);
    
    // DECREASE STOCK
    product.StockQuantity -= cartItem.Quantity;
    _productRepo.Update(product);
}
```

**Why Store Price in OrderItem?**
- Product prices may change in future
- Order must reflect the price customer paid
- Historical accuracy for accounting/reports

**How Stock is Decreased:**
1. Get current product from database
2. Subtract ordered quantity: `product.StockQuantity -= cartItem.Quantity`
3. Update product in database
4. If transaction fails, stock is automatically restored (rollback)

#### **Step F: Save All Changes**
```csharp
_orderRepo.Save();           // Save order items
transaction.Commit();        // Make all changes permanent
```
- All database changes are committed together
- If `Commit()` fails, everything is rolled back

#### **Step G: Clear Cart from Session**
```csharp
CartService.ClearCart(HttpContext.Session);
```

**Why Clear Cart After Checkout?**
- Cart items are now in database as Order
- User should start fresh for next purchase
- Prevents accidental re-ordering
- `ClearCart()` removes the "ShoppingCart" key from session

#### **Step H: Redirect to Confirmation**
```csharp
return RedirectToAction("Confirmation", new { orderNumber });
```
- User sees order confirmation page
- Order number displayed for tracking

---

## 🔄 Data Transformation: CartItem → OrderItem

### CartItem (Session Storage)
```csharp
CartItem (in Session as JSON)
{
    ProductId: 5,
    ProductName: "Laptop",
    Price: 999.99,
    Quantity: 2,
    PictureUrl: "laptop.jpg",
    TotalPrice: 1999.98
}
```

### ⬇️ Converted To ⬇️

### OrderItem (Database Record)
```csharp
OrderItem (in SQL Database)
{
    OrderItemId: AUTO,
    OrderId: 123,           // Links to Order
    ProductId: 5,           // Links to Product
    UnitPrice: 999.99,      // Price at purchase
    Quantity: 2,
    LineTotal: 1999.98
}
```

---

## 🗄️ Database Transaction Flow

### Without Transaction (❌ BAD)
```
1. Create Order        ✅ SUCCESS
2. Create OrderItem 1  ✅ SUCCESS  
3. Create OrderItem 2  ✅ SUCCESS
4. Decrease Stock 1    ✅ SUCCESS
5. Decrease Stock 2    ❌ FAILS (out of stock error)

Result: Order exists with items, but stock not updated correctly → INCONSISTENT DATA
```

### With Transaction (✅ GOOD)
```
BEGIN TRANSACTION
1. Create Order        ✅ SUCCESS
2. Create OrderItem 1  ✅ SUCCESS
3. Create OrderItem 2  ✅ SUCCESS  
4. Decrease Stock 1    ✅ SUCCESS
5. Decrease Stock 2    ❌ FAILS
ROLLBACK TRANSACTION

Result: ALL changes undone, database unchanged → CONSISTENT DATA
```

---

## 📊 Order Status Values

| Status | Meaning | Description |
|--------|---------|-------------|
| 0 | Pending | Order placed, payment pending |
| 1 | Processing | Order confirmed, being prepared |
| 2 | Shipped | Order shipped to customer |
| 3 | Delivered | Order delivered successfully |
| 4 | Cancelled | Order cancelled |

---

## 🔐 Security Features

✅ **Anti-forgery tokens** on all POST requests  
✅ **Model validation** ensures required fields  
✅ **Stock validation** prevents overselling  
✅ **Transaction rollback** on errors  
✅ **Session isolation** - each user has own cart  

---

## 🎯 Complete Checkout Process Summary

```
1. User browses Catalog → Adds items to Cart (Session)
                          ↓
2. User views Cart → Reviews items
                          ↓
3. User clicks "Proceed to Checkout"
                          ↓
4. Checkout page loads → Shows cart + address selection
                          ↓
5. User selects address → Clicks "Place Order"
                          ↓
6. BEGIN TRANSACTION
   - Create Order in database
   - Convert each CartItem → OrderItem
   - Decrease stock for each product
   - Commit transaction
                          ↓
7. Clear cart from Session
                          ↓
8. Redirect to Confirmation page → Show order details
```

---

## 📁 Files Created

### Controllers:
- `OrdersController.cs` - Handles checkout, confirmation, order details

### Repositories:
- `IOrderRepo.cs` - Order repository interface
- `OrderRepo.cs` - Order repository implementation
- `IAddressRepo.cs` - Address repository interface  
- `AddressRepo.cs` - Address repository implementation

### ViewModels:
- `CheckoutVM.cs` - Checkout page data
- `OrderDetailsVM.cs` - Order confirmation/details data

### Views:
- `Orders/Checkout.cshtml` - Checkout page
- `Orders/Confirmation.cshtml` - Order success page
- `Orders/Details.cshtml` - Order details page
- `Orders/Index.cshtml` - All orders list

---

## 🚀 Testing the Module

1. **Add items to cart** from Catalog
2. **View cart** - items should display
3. **Click "Proceed to Checkout"**
4. **Select shipping address** from dropdown
5. **Click "Place Order"**
6. **See confirmation page** with order number
7. **Check Orders page** - new order appears
8. **Verify stock decreased** in Products admin

---

## 💡 Key Design Decisions

### Why Session for Cart?
- Temporary data (doesn't need persistence)
- Fast access (no database queries)
- Works without login

### Why Database for Orders?
- Permanent records needed
- Inventory tracking required
- Business reporting essential

### Why Transaction?
- Data consistency crucial for e-commerce
- Prevents inventory discrepancies
- Ensures reliable order processing


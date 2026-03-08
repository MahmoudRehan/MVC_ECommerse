# Test Data Setup for Orders Module

## 📦 Adding Test Addresses

Since the checkout requires shipping addresses, you'll need to add some test addresses to the database.

### Option 1: SQL Script (Run in SQL Server Management Studio or Azure Data Studio)

```sql
USE [YourDatabaseName];

-- Insert test shipping addresses
INSERT INTO Addresses (Street, City, Zip, Country, IsDefault)
VALUES 
    ('123 Main Street', 'New York', '10001', 'USA', 1),
    ('456 Oak Avenue', 'Los Angeles', '90001', 'USA', 0),
    ('789 Elm Road', 'Chicago', '60601', 'USA', 0),
    ('321 Pine Lane', 'Houston', '77001', 'USA', 0);
```

### Option 2: C# Seed Data (Add to Program.cs or create a seeder class)

```csharp
// In Program.cs, before app.Run():
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    if (!context.Addresses.Any())
    {
        context.Addresses.AddRange(
            new Address
            {
                Street = "123 Main Street",
                City = "New York",
                Zip = "10001",
                Country = "USA",
                IsDefault = true
            },
            new Address
            {
                Street = "456 Oak Avenue",
                City = "Los Angeles",
                Zip = "90001",
                Country = "USA",
                IsDefault = false
            },
            new Address
            {
                Street = "789 Elm Road",
                City = "Chicago",
                Zip = "60601",
                Country = "USA",
                IsDefault = false
            }
        );
        
        context.SaveChanges();
    }
}
```

### Option 3: Using Package Manager Console

```powershell
# Create a migration to seed data
Add-Migration SeedAddresses
Update-Database
```

Then create the migration file with seed data.

---

## 🧪 Testing the Complete Checkout Flow

### Prerequisites:
✅ Database with Products (with stock > 0)  
✅ Database with Categories  
✅ Database with Addresses (at least 1)  

### Test Steps:

1. **Browse Catalog**
   - Navigate to "🛍️ Shop" in menu
   - You should see active products

2. **Add Items to Cart**
   - Click "Add to Cart" on 2-3 different products
   - Click "🛒 Cart" in navigation
   - Verify items appear in cart

3. **Proceed to Checkout**
   - Click "Proceed to Checkout" button
   - Checkout page should load with:
     - Cart items summary
     - Address dropdown (populated with addresses)
     - Total amount

4. **Place Order**
   - Select a shipping address
   - Click "Place Order"
   - Should redirect to Confirmation page showing:
     - Order number (e.g., ORD-20260305-1234)
     - Order items
     - Shipping address
     - Total amount

5. **Verify Results**
   - Cart should be empty (cleared after checkout)
   - Navigate to "📋 Orders" → order should appear in list
   - Check Products admin → stock should be decreased

6. **Verify Transaction Rollback (Error Handling)**
   - Manually set a product stock to 0 in database
   - Try to checkout with that product in cart
   - Should show error message
   - Cart should remain intact (not cleared)
   - No order created in database

---

## 🔍 Troubleshooting

### "Your cart is empty" error on checkout
- Cart session may have expired (30 min timeout)
- Solution: Add items to cart again

### "Selected address is invalid" error
- No addresses in database
- Solution: Add addresses using SQL script above

### "Not enough stock" error during checkout
- Product stock was reduced after adding to cart
- Solution: Update cart quantities or remove item

### Order created but stock not decreased
- Transaction not working properly
- Check: `app.UseSession()` is called in Program.cs
- Check: Transaction commit/rollback logic in controller

---

## 📊 Database Changes After Checkout

### Before Checkout:
```
Products Table:
- Laptop: Stock = 10

Cart (Session):
- Laptop: Qty = 2

Orders Table: (empty)
OrderItems Table: (empty)
```

### After Checkout:
```
Products Table:
- Laptop: Stock = 8 (decreased by 2)

Cart (Session): (cleared)

Orders Table:
- Order #1: OrderNumber=ORD-20260305-1234, Total=$1999.98

OrderItems Table:
- Item #1: OrderId=1, ProductId=5, Qty=2, LineTotal=$1999.98
```

---

## 🔐 Security Considerations

✅ **Anti-forgery token** prevents CSRF attacks  
✅ **Stock validation** prevents overselling  
✅ **Transaction rollback** prevents data corruption  
✅ **Model validation** ensures required fields  
✅ **Session timeout** (30 min) prevents stale carts  

---

## 🎯 Next Steps (Future Enhancements)

- [ ] Add payment gateway integration
- [ ] Send order confirmation email
- [ ] Add order status tracking
- [ ] Implement order cancellation
- [ ] Add invoice generation
- [ ] Add customer order history (requires authentication)
- [ ] Add admin order management dashboard


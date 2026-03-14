using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PP.Repos.Inerfaces;
using PP.ViewModels;

namespace PP.Areas.Admin.Controllers
{
    
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : Controller
    {
        private readonly IOrderRepo _orderRepo;

        public AdminOrderController(IOrderRepo orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public IActionResult Index()
        {
            return View(_orderRepo.GetAllOrders());
        }

        public IActionResult Details(int id)
        {
            var order = _orderRepo.GetOrderWithDetails(id);
            if (order == null)
                return NotFound();

            var ordervm = new OrderDetailsVM
            {
                Id = order.OrderId,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                OrderItems = order.OrderItems.ToList(),
                CustomerEmail = order.User?.Email
            };
            return View(ordervm);
        }

        [HttpGet]
        public IActionResult UpdateStatus(int id)
        {
            var order = _orderRepo.GetOrderWithDetails(id);
            if (order == null)
                return NotFound();
            return View(order);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, int newStatus)
        {
            var order = _orderRepo.GetOrderWithDetails(id);
            if (order == null)
                return NotFound();

            order.Status = newStatus;
            _orderRepo.Update(order);
            _orderRepo.Save();
            return RedirectToAction("Index");
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PP.Repos.Inerfaces;
using PP.ViewModels;

namespace PP.Areas.Admin.Controllers
{
   
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IProductRepo _productRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IOrderRepo _orderRepo;

        public DashboardController(IProductRepo productRepo, ICategoryRepo categoryRepo, IOrderRepo orderRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _orderRepo = orderRepo;
        }

        public IActionResult Index()
        {
            var products = _productRepo.GetAll();
            var categories = _categoryRepo.GetAll();
            var orders = _orderRepo.GetAllOrders();
            var completedOrders = _orderRepo.GetAllCompletedOrders();
            var pendingOrders = _orderRepo.GetAllPendingOrders();

            var vm = new DashboardVM
            {
                TotalProducts = products.Count(),
                TotalCategories = categories.Count(),
                TotalOrders = orders.Count(),
                CancelledOrders = orders.Count(o => o.Status == 4),
                CompletedOrders = completedOrders.Count(),
                PendingOrders = pendingOrders.Count(),
                Revenue = completedOrders.Sum(o => o.TotalAmount),
                Products = products,
                Categories = categories,
                Orders = orders
            };

            return View(vm);
        }
    }
}

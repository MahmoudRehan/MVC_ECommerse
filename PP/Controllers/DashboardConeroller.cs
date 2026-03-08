using Microsoft.AspNetCore.Mvc;
using PP.Repos.Inerfaces;
using PP.ViewModels;
namespace PP.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IProductRepo _ProductRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IOrderRepo orderRepo;

        public DashboardController(IProductRepo productRepo, ICategoryRepo categoryRepo , IOrderRepo orderRepo )
        {
            _ProductRepo = productRepo;
            _categoryRepo = categoryRepo;
            this.orderRepo = orderRepo;
        }


        public IActionResult Index()
        {
            var Products = _ProductRepo.GetAll();
            var categories = _categoryRepo.GetAll();
            var orders = orderRepo.GetAllOrders();
            var completedOrders = orderRepo.GetAllCompletedOrders();
            var pendingOrders = orderRepo.GetAllPendingOrders();

            
           
      

        

            var DashboardVM = new DashboardVM
            {
                TotalProducts = Products.Count(),
                TotalCategories = categories.Count(),
                TotalOrders = orders.Count(),
                CancelledOrders = orders.Count(o => o.Status == 4),
                CompletedOrders = completedOrders.Count(),
                PendingOrders = pendingOrders.Count(),
                Revenue = completedOrders.Sum(o => o.TotalAmount),
                Products = Products,
                Categories = categories,
                Orders = orders
            };

            return View(DashboardVM);
        }
    }
}

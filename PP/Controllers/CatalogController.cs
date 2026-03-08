using Microsoft.AspNetCore.Mvc;
using PP.Repos.Inerfaces;

namespace PP.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IProductRepo _productRepo;
        private readonly ICategoryRepo _categoryRepo;

        public CatalogController(IProductRepo productRepo, ICategoryRepo categoryRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
        }

        public IActionResult Index(string? q, int? categoryId, string? sort)
        {
            var products = _productRepo.GetActiveProducts(q, categoryId, sort);

            ViewBag.SearchQuery = q;
            ViewBag.CategoryId = categoryId;
            ViewBag.Sort = sort;
            ViewBag.Categories = _categoryRepo.GetAll();

            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _productRepo.GetActiveProductById(id);
            
            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PP.Models;
using PP.Repos.Inerfaces;
using PP.ViewModels;

namespace PP.Areas.Admin.Controllers
{
   
    // -----------------------------------------------------------------------
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        public readonly IProductRepo _productRepo;
        private readonly ICategoryRepo _categoryRepo;

        public ProductsController(IProductRepo productRepo, ICategoryRepo categoryRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
        }

        public IActionResult Index(string? q, int? categoryId)
        {
            var products = _productRepo.FilterProducts(q, categoryId);

            ViewBag.SearchQuery = q;
            ViewBag.CategoryId = categoryId;
            ViewBag.Categories = _categoryRepo.GetAll();

            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null)
                return NotFound();
            return View(product);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _categoryRepo.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryRepo.GetAll();
                return View(productVM);
            }

            string fileName = "LEO.jpg";
            if (productVM.Image != null)
                fileName = DocumentSettings.UploadFile(productVM.Image, "Products");

            var product = new Product
            {
                Name = productVM.Name,
                SKU = productVM.SKU,
                Price = productVM.Price,
                StockQuantity = productVM.StockQuantity,
                PictureUrl = fileName,
                CategoryId = productVM.CategoryId,
                IsActive = productVM.IsActive
            };

            _productRepo.Add(product);
            _productRepo.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null)
                return NotFound();

            var vm = new ProductVM
            {
                Id = product.Id,
                Name = product.Name,
                SKU = product.SKU,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                IsActive = product.IsActive,
                ExistingImage = product.PictureUrl
            };

            ViewBag.Categories = _categoryRepo.GetAll();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _categoryRepo.GetAll();
                return View(vm);
            }

            var product = _productRepo.GetById(vm.Id);
            if (product == null)
                return NotFound();

            product.Name = vm.Name;
            product.SKU = vm.SKU;
            product.Price = vm.Price;
            product.StockQuantity = vm.StockQuantity;
            product.CategoryId = vm.CategoryId;
            product.IsActive = vm.IsActive;

            if (vm.Image != null)
            {
                if (!string.IsNullOrEmpty(product.PictureUrl))
                    DocumentSettings.DeleteFile(product.PictureUrl, "Products");

                product.PictureUrl = DocumentSettings.UploadFile(vm.Image, "Products");
            }
            else
            {
                product.PictureUrl = vm.ExistingImage;
            }

            _productRepo.Update(product);
            _productRepo.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null)
                return NotFound();

            if (!string.IsNullOrEmpty(product.PictureUrl))
                DocumentSettings.DeleteFile(product.PictureUrl, "Products");

            _productRepo.Delete(id);
            _productRepo.Save();
            return RedirectToAction("Index");
        }

        public IActionResult ProductsByCategory(int id, string? q, int? categoryId)
        {
            var products = _productRepo.GetProductsBerCategory(categoryId ?? id);

            ViewBag.SearchQuery = q;
            ViewBag.CategoryId = categoryId ?? id;
            ViewBag.Categories = _categoryRepo.GetAll();

            return View(products);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using PP.Models;
using PP.Repos.Inerfaces;
using PP.ViewModels;

namespace PP.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepo _cat;

        public CategoriesController(ICategoryRepo cat)
        {
            _cat = cat;
        }
        
        public IActionResult Index()
        {
            var categories = _cat.GetAll();
            return View(categories);
        }
        
        public IActionResult Details(int id)
        {
            var category = _cat.GetById(id);
            if (category == null)
                return NotFound();
            
            return View(category);
        }
        
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoryVM categoryVM)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryVM);
            }
            
            Category category = new Category
            {
                Name = categoryVM.Name
            };
            
            _cat.Add(category);
            _cat.Save();
            
            return RedirectToAction("Index");
        }
        
        public IActionResult Edit(int id)
        {
            var category = _cat.GetById(id);
            
            if (category == null)
                return NotFound();
            
            CategoryVM vm = new CategoryVM
            {
                Id = category.Id,
                Name = category.Name
            };
            
            return View(vm);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            
            var category = _cat.GetById(vm.Id);
            
            if (category == null)
                return NotFound();
            
            category.Name = vm.Name;
            
            _cat.Update(category);
            _cat.Save();
            
            return RedirectToAction("Index");
        }
        
        public IActionResult Delete(int id)
        {
            var category = _cat.GetById(id);
            if (category == null)
                return NotFound();
            
            _cat.Delete(id);
            _cat.Save();
            
            return RedirectToAction("Index");
        }
    }
}

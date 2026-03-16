using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PP.Models;
using PP.Repos.Inerfaces;
using PP.ViewModels;

namespace PP.Areas.Admin.Controllers
{
   
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepo _cat;

        public CategoriesController(ICategoryRepo cat)
        {
            _cat = cat;
        }

        public IActionResult Index()
        {
            return View(_cat.GetAll());
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
            var categoryExists = _cat.GetAll().Any(c => c.Name == categoryVM.Name);
            if (categoryExists)
            {
                ModelState.AddModelError("Name", "A category with this name already exists.");
            }
            if (!ModelState.IsValid)
                return View(categoryVM);

            var category = new Category { Name = categoryVM.Name };
            _cat.Add(category);
            _cat.Save();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var category = _cat.GetById(id);
            if (category == null)
                return NotFound();

            var vm = new CategoryVM { Id = category.Id, Name = category.Name };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

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

        public IActionResult Deleted()
        {
            return View(_cat.GetDeletedCategories());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Restore(int id)
        {
            var category = _cat.GetDeletedCategoryById(id);
            if (category == null)
                return NotFound();

            _cat.Restore(id);
            _cat.Save();

            return RedirectToAction("Deleted");
        }
    }
}

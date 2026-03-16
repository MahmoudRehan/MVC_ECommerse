using PP.Models;
using PP.Models.Context;
using PP.Repos.Inerfaces;

namespace PP.Repos.Implementation
{
    public class CategoryRepo: GenericRepo<Category>, ICategoryRepo
    {
        private readonly AppDbContext _context;

        public CategoryRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Category> GetDeletedCategories()
        {
            return _context.Categories
                .Where(c => c.IsDeleted)
                .ToList();
        }

        public Category GetDeletedCategoryById(int id)
        {
            return _context.Categories
                .FirstOrDefault(c => c.Id == id && c.IsDeleted);
        }

        public void Restore(int id)
        {
            var category = GetDeletedCategoryById(id);
            if (category == null)
                return;

            category.IsDeleted = false;
            _context.Categories.Update(category);
        }
    }
}

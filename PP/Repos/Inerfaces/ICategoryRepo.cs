using PP.Models;

namespace PP.Repos.Inerfaces
{
    public interface ICategoryRepo: IGenericRepo<Category>
    {
        public IEnumerable<Category> GetDeletedCategories();
        public Category GetDeletedCategoryById(int id);
        public void Restore(int id);
    }
}

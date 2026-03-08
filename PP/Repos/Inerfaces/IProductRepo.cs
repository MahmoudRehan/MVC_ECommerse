using PP.Models;

namespace PP.Repos.Inerfaces
{
    public interface IProductRepo: IGenericRepo<Product>
    {

        public IEnumerable<Product> FilterProducts(string? q, int? categoryId);
        public IEnumerable<Product> GetProductsBerCategory(int  categoryId);
        public IEnumerable<Product> GetActiveProducts(string? q, int? categoryId, string? sort);
        public Product GetActiveProductById(int id);
    }
}

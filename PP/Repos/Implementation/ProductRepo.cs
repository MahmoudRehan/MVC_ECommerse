using Microsoft.EntityFrameworkCore;
using PP.Models;
using PP.Models.Context;
using PP.Repos.Inerfaces;

namespace PP.Repos.Implementation
{
    public class ProductRepo: GenericRepo<Product>, IProductRepo
    {
        private readonly AppDbContext _context;

        public ProductRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }


        public IEnumerable<Product> FilterProducts(string? q, int? categoryId)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                q = q.ToLower();

                query = query.Where(p =>
                    p.Name.ToLower().Contains(q) ||
                    p.SKU.ToLower().Contains(q));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            return query.ToList();
        }

        public IEnumerable<Product> GetProductsBerCategory(int categoryId)
        {
            return _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToList();
        }

        public IEnumerable<Product> GetActiveProducts(string? q, int? categoryId, string? sort)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                q = q.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(q));
            }

            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            query = sort?.ToLower() switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Name)
            };

            return query.ToList();
        }

        public Product GetActiveProductById(int id)
        {
            return _context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id && p.IsActive);
        }
    }
}

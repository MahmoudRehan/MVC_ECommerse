using Microsoft.EntityFrameworkCore;
using PP.Models;
using PP.Models.Context;
using PP.Repos.Inerfaces;

namespace PP.Repos.Implementation
{
    public class GenericRepo<T> : IGenericRepo<T> where T : class
    {
        private readonly AppDbContext _context;

        public GenericRepo(AppDbContext context)
        {
            _context = context;
        } 
        public IEnumerable<T> GetAll()
        {
            if(typeof(T) == typeof(Product))
            {
                return _context.Products.Include(p => p.Category).ToList() as IEnumerable<T>;
            }

            return _context.Set<T>().ToList();

        }
        public T GetById(int id)
        {
            if (typeof(T) == typeof(Product)) 
            {
               return _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id) as T;
            }
            return _context.Set<T>().Find(id);
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);  
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }
        public void Delete(int id)
        {
            var entity = GetById(id);

            if (entity != null)
                _context.Set<T>().Remove(entity);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}

namespace PP.Repos.Inerfaces
{
    public interface IGenericRepo<T>
    {
        public IEnumerable<T> GetAll();

        public T GetById(int id);

        public void Add(T entity);
        public void Update(T entity);
        public void Delete(int id);

        void Save();
    }
}

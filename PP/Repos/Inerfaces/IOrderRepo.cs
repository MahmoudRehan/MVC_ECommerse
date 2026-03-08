using PP.Models;

namespace PP.Repos.Inerfaces
{
   
    public interface IOrderRepo : IGenericRepo<Order>
    {
       
        Order GetOrderWithDetails(int orderId);

      
        Order GetOrderByNumber(string orderNumber);

       
        IEnumerable<Order> GetAllOrders();
        public IEnumerable<Order> GetAllPendingOrders();
        public IEnumerable<Order> GetAllCompletedOrders();
    }
}

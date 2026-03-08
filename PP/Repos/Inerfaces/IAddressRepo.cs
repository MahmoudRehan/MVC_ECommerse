using PP.Models;

namespace PP.Repos.Inerfaces
{
   
    public interface IAddressRepo : IGenericRepo<Address>
    {
       
        Address GetDefaultAddress();
    }
}

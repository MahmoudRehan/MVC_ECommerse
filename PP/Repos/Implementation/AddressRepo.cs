using PP.Models;
using PP.Models.Context;
using PP.Repos.Inerfaces;

namespace PP.Repos.Implementation
{
   
    public class AddressRepo : GenericRepo<Address>, IAddressRepo
    {
        private readonly AppDbContext _context;

        public AddressRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }

       
        public Address GetDefaultAddress()
        {
            // Try to get the address marked as default
            var defaultAddress = _context.Addresses.FirstOrDefault(a => a.IsDefault);

            // If no default address, return the first address
            return defaultAddress ?? _context.Addresses.FirstOrDefault();
        }
    }
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PP.Models
{
    public class AppUser : IdentityUser
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; }
        [Required, MaxLength(100)]
        public string LastName { get; set; }

        public string FullName => FirstName + " " + LastName;
        public virtual ICollection<Address> Addresses { get; set; } = new HashSet<Address>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}

namespace PP.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public bool IsDefault { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}

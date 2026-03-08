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

        // Navigation property for the related User
        //public int UserId { get; set; }
        //public User User { get; set; }
    }
}

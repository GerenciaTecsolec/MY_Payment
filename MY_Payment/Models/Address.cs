namespace MY_Payment.Models
{
    public class Address
    {
        public Guid id { get; set; }
        public string? reference { get; set; }
        public string description { get; set; } = string.Empty;
        public string longitude { get; set; } = string.Empty;
        public string latitude { get; set; } = string.Empty;
        public bool principal { get; set; }
    }
}

namespace MY_Payment.Models
{
    public class Client
    {
        public Guid id { get; set; }
        public string name { get; set; } = string.Empty;
        public string? paternalSurname { get; set; }
        public string? maternalSurname { get; set; }
        public string? businessName { get; set; }
        public string? identificationType { get; set; }
        public string? identification { get; set; }
        public string phoneNumber { get; set; } = string.Empty;
        public string? countryCode { get; set; } = string.Empty;
        public string? email { get; set; }
        public DateTime? birthday { get; set; }
        public List<Address>? addresses { get; set; }
    }
}

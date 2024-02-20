namespace MY_Payment.Models
{
    public class CardBrand
    {
        public Guid id { get; set; }
        public string cardType { get; set; } = string.Empty;
        public string brand { get; set; } = string.Empty;
        public string logo { get; set; } = string.Empty;
        public DateTime creationDate { get; set; }
    }
}

namespace MY_Payment.Models
{
    public class ClientCard
    {
        public string holderName { get; set; } = string.Empty;
        public string number { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
        public CardBrand cardBrand { get; set; }
    }
}

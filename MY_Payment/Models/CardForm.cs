using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class CardForm
    {
        public string? holderName { get; set; }
        public string? number { get; set; }
        public string? cardBrand { get; set; }
        public string? cardLogo { get; set; }
    }
}

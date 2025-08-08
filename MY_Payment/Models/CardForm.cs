using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class CardForm
    {
        [JsonProperty("holderName")]
        public string? HolderName { get; set; }

        [JsonProperty("number")]
        public string? Number { get; set; }

        [JsonProperty("cardBrand")]
        public string? CardBrand { get; set; }

        [JsonProperty("cardLogo")]
        public string? CardLogo { get; set; }
    }
}

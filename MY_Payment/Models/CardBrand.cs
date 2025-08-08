using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class CardBrand
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("cardType")]
        public string CardType { get; set; } = string.Empty;

        [JsonProperty("brand")]
        public string Brand { get; set; } = string.Empty;

        [JsonProperty("logo")]
        public string Logo { get; set; } = string.Empty;

        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }
    }
}

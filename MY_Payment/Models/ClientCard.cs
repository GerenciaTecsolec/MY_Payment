using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class ClientCard
    {
        [JsonProperty("holderName")]
        public string HolderName { get; set; } = string.Empty;

        [JsonProperty("number")]
        public string Number { get; set; } = string.Empty;

        [JsonProperty("token")]
        public string Token { get; set; } = string.Empty;

        [JsonProperty("cardBrand")]
        public CardBrand CardBrand { get; set; }
    }
}

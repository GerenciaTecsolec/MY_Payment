using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class CardDebit
    {

        [JsonProperty("bin")]
        public long Bin { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("expiry_year")]
        public long ExpiryYear { get; set; }

        [JsonProperty("expiry_month")]
        public long ExpiryMonth { get; set; }

        [JsonProperty("transaction_reference")]
        public string TransactionReference { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("number")]
        public long Number { get; set; }

        [JsonProperty("origin")]
        public string Origin { get; set; }
    }
}

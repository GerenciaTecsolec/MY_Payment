using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class Card
    {
        [JsonProperty("bin")]
        public string bin { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("token")]
        public string token { get; set; }

        [JsonProperty("holder_name")]
        public string? holderName { get; set; }

        [JsonProperty("expiry_year")]
        public string expiryYear { get; set; }

        [JsonProperty("expiry_month")]
        public string expiryMonth { get; set; }

        [JsonProperty("transaction_reference")]
        public string transactionReference { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("number")]
        public string number { get; set; }

        [JsonProperty("message")]
        public string? message { get; set; }

        [JsonProperty("origin")]
        public string origin { get; set; }

        [JsonProperty("fiscal_number")]
        public string? fiscalNumber { get; set; }
    }
}

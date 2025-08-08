using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class PaymentResume
    {
        [JsonProperty("addressDescription")]
        public string? AddressDescription { get; set; }

        [JsonProperty("addressReference")]
        public string? AddressReference { get; set; }

        [JsonProperty("averageTime")]
        public decimal AverageTime { get; set; }

        [JsonProperty("subtotal")]
        public decimal Subtotal { get; set; }

        [JsonProperty("deliveryFee")]
        public decimal DeliveryFee { get; set; }

        [JsonProperty("service")]
        public decimal Service { get; set; }

        [JsonProperty("total")]
        public decimal Total { get; set; }

    }
}

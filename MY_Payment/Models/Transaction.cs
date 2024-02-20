using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class Transaction
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("current_status")]
        public string CurrentStatus { get; set; }

        [JsonProperty("status_detail")]
        public int StatusDetail { get; set; }

        [JsonProperty("payment_date")]
        public DateTime PaymentDate { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("installments")]
        public long Installments { get; set; }

        [JsonProperty("carrier_code")]
        public string CarrierCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("authorization_code")]
        public string AuthorizationCode { get; set; }

        [JsonProperty("dev_reference")]
        public Guid DevReference { get; set; }

        [JsonProperty("carrier")]
        public string Carrier { get; set; }

        [JsonProperty("product_description")]
        public string ProductDescription { get; set; }

        [JsonProperty("payment_method_type")]
        public string PaymentMethodType { get; set; }

        [JsonProperty("installment_type")]
        public string InstallmentType { get; set; }
    }
}

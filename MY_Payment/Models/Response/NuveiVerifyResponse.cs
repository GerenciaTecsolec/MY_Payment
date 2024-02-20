using Newtonsoft.Json;

namespace MY_Payment.Models.Response
{
    public class NuveiVerifyResponse
    {
        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("status_detail")]
        public int StatusDetail { get; set; }

        [JsonProperty("payment_date")]
        public string PaymentDate { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}

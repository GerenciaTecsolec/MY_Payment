using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class PaymentResumeResponse
    {
        [JsonProperty("error")]
        public required bool Error { get; set; }

        [JsonProperty("resultCode")]
        public required int ResultCode { get; set; }

        [JsonProperty("resultMessage")]
        public required PaymentResume ResultMessage { get; set; }
    }
}

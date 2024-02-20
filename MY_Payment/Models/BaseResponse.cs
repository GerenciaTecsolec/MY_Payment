using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class BaseResponse
    {
        [JsonProperty("error")]
        public bool Error { get; set; }

        [JsonProperty("resultCode")]
        public long ResultCode { get; set; }

        [JsonProperty("resultMessage")]
        public string ResultMessage { get; set; }

    }
}

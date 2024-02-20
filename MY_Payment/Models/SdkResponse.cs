using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class SdkResponse
    {
        [JsonProperty("acs_trans_id")]
        public string AcsTransId { get; set; }

        [JsonProperty("acs_signed_content")]
        public string AcsSignedContent { get; set; }

        [JsonProperty("acs_reference_number")]
        public string AcsReferenceNumber { get; set; }
    }
}

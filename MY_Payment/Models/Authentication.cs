using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class Authentication
    {
        [JsonProperty("cavv")]
        public string Cavv { get; set; }

        [JsonProperty("xid")]
        public string Xid { get; set; }

        [JsonProperty("eci")]
        public string Eci { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("return_code")]
        public string ReturnCode { get; set; }

        [JsonProperty("return_message")]
        public string ReturnMessage { get; set; }

        [JsonProperty("reference_id")]
        public string ReferenceId { get; set; }

        [JsonProperty("service")]
        public string Service { get; set; }
    }
}

using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class ThreeDs2Data
    {
        [JsonProperty("term_url")]
        public string TermUrl { get; set; }

        [JsonProperty("device_type")]
        public string DeviceType { get; set; }

        [JsonProperty("process_anyway")]
        public bool ProcessAnyway { get; set; }
    }
}

using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class ExtraParams
    {
        [JsonProperty("threeDS2_data")]
        public ThreeDs2Data threeDs2Data { get; set; }

        [JsonProperty("browser_info")]
        public BrowserInfo BrowserInfoData { get; set; }
    }
}

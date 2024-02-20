using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class ThreeDs
    {
        [JsonProperty("authentication")]
        public Authentication Authentication { get; set; }

        [JsonProperty("browser_response")]
        public BrowserResponse BrowserResponse { get; set; }

        [JsonProperty("sdk_response")]
        public SdkResponse SdkResponse { get; set; }
    }
}

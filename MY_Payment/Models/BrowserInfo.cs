using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class BrowserInfo
    {
        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("java_enabled")]
        public bool JavaEnabled { get; set; }

        [JsonProperty("js_enabled")]
        public bool JsEnabled { get; set; }

        [JsonProperty("color_depth")]
        public int ColorDepht { get; set; }

        [JsonProperty("screen_height")]
        public int ScreenHeight { get; set; }

        [JsonProperty("screen_width")]
        public int ScreenWidth { get; set; }

        [JsonProperty("timezone_offset")]
        public int TimezoneOffset { get; set; }

        [JsonProperty("user_agent")]
        public string UserAgent { get; set; }

        [JsonProperty("accept_header")]
        public string AcceptHeader { get; set; }
    }
}

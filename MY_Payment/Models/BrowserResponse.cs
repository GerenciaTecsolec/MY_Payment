using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class BrowserResponse
    {
        [JsonProperty("challenge_request")]
        public string ChallengeRequest { get; set; }

        [JsonProperty("hidden_iframe")]
        public string HiddenIframe { get; set; }
    }
}

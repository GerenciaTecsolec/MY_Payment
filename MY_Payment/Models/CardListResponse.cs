using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class CardListResponse
    {
        [JsonProperty("cards")]
        public List<Card> cards { get; set; }

        [JsonProperty("result_size")]
        public int size { get; set; }
    }
}

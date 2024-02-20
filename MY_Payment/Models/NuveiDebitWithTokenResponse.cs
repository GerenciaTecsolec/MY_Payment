using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class NuveiDebitWithTokenResponse
    {
        [JsonProperty("transaction")]
        public Transaction Transaction { get; set; }

        [JsonProperty("card")]
        public Card Card { get; set; }

        [JsonProperty("3ds")]
        public ThreeDs ThreeDs { get; set; }

    }
}

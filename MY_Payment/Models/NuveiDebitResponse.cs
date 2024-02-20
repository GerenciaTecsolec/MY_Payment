using Newtonsoft.Json;
using System.Transactions;

namespace MY_Payment.Models
{
    public class NuveiDebitResponse
    {
        [JsonProperty("transaction")]
        public Transaction Transaction { get; set; }

        [JsonProperty("card")]
        public CardDebit Card { get; set; }
    }
}

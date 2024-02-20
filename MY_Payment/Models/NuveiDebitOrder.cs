using Newtonsoft.Json;

namespace MY_Payment.Models
{
    public class NuveiDebitOrder
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("dev_reference")]
        public string DevReference { get; set; }

        [JsonProperty("vat")]
        public decimal Vat { get; set; }

        [JsonProperty("tax_percentage")]
        public int TaxPercentage { get; set; }

        [JsonProperty("taxable_amount")]
        public decimal TaxableAmount { get; set; }
    }
}

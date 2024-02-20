namespace MY_Payment.Models
{
    public class NuveiDebitRequest
    {
        public NuveiDebitUser user { get; set; }
        public NuveiDebitOrder order { get; set; }
        public NuveiDebitCard card { get; set; }
        public ExtraParams extra_params { get; set; }
    }
}

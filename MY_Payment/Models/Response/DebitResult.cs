namespace MY_Payment.Models.Response
{
    public class DebitResult
    {
        public bool error { get; set; }
        public string resultCode { get; set; }
        public string? codeStatus { get; set; }
        public string? iframe { get; set; }
        public string? order { get; set; }
        public string? transactionId { get; set; }
        public string? secuence { get; set; }
    }
}

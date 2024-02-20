namespace MY_Payment.Models
{
    public class NuveiBrowser
    {
        public Guid id { get; set; }
        public Guid TransactionId { get; set; }
        public string nuveiTransactionId { get; set; }
        public string? challengeRequest { get; set; }
        public string? hiddenIframe { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime? updatedDate { get; set; }
    }

    public class NuveiTransaction
    {
        public Guid id { get; set; }
        public Guid orderId { get; set; }
        public string nuveiTransactionId { get; set; }
        public string? status { get; set; }
        public string? currentStatus { get; set; }
        public int? statusDetail { get; set; }
        public DateTime? paymentDate { get; set; }
        public decimal amount { get; set; }
        public int? installment { get; set; }
        public string? carrierCode { get; set; }
        public string? message { get; set; }
        public string? authorizationCode { get; set; }
        public string? devReference { get; set; }
        public string? carrier { get; set; }
        public string? productDescription { get; set; }
        public string? paymentMethodType { get; set; }
        public string? installmentType { get; set; }
        public DateTime creationDate { get; set; }
        public DateTime? updatedDate { get; set; }
    }

    public class NuveiTransactionFull
    {
        public NuveiTransaction? transaction { get; set; }
        public NuveiBrowser? browserInfo { get; set; }
    }


}

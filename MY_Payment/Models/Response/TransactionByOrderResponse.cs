namespace MY_Payment.Models.Response
{
    public class TransactionByOrderResponse: BaseResponse
    {
        public NuveiTransactionFull result { get; set; }
    }
}

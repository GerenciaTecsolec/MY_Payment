using Newtonsoft.Json;

namespace MY_Payment.Models.Response
{
    public class ConfirmOrderResponse:BaseResponse
    {
        [JsonProperty("order")]
        public Order? Order { get; set; }
    }


    public partial class Order
    {
        [JsonProperty("id")]
        public Guid? Id { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }
    }
}


namespace MY_Payment.Models
{
    public class NuveiDebitUser
    {
        public string id { get; set; }
        public string email { get; set; }

        public static implicit operator NuveiDebitUser(NuveiDebitOrder v)
        {
            throw new NotImplementedException();
        }
    }
}

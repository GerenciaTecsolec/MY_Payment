namespace MY_Payment.Models.Request
{
    public class EmailRequest
    {
        public string sendTo { get; set; } = string.Empty;
        public string subject { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
        public string copyTo { get; set; } = string.Empty;
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MY_Payment.Models;
using MY_Payment.Service;

namespace MY_Payment.Controllers
{
    public class VerifyController : Controller
    {
        AuthService authService;
        ClientService clientService;
        private IConfiguration configuration;
        PaymentService paymentService;

        public VerifyController(IConfiguration _configuration)
        {
            this.configuration = _configuration;
            paymentService = new PaymentService(configuration);
            clientService = new ClientService(configuration);
            authService = new AuthService(configuration);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                string hostUrl = configuration.GetValue<string>("globalVariables:hostUrlAdmin")!;
                ViewBag.hostUrl = hostUrl;
                var queryString = HttpContext.Request.QueryString!.ToString();
                if (!string.IsNullOrEmpty(queryString))
                {
                    string sessionId = HttpContext.Request.Query["ts"].ToString();
                    string orderId = HttpContext.Request.Query["order"].ToString();
                    string cresId = HttpContext.Request.Query["cres"].ToString();
                    string secuence = HttpContext.Request.Query["sec"].ToString();
                    string tokenApp = await authService.GenerateTokenApplication();
                    string tokenSession = await authService.GetCurrentTokenSession(sessionId, tokenApp);
                    Client? client = await clientService.GetClientInfo(tokenSession, tokenApp);
                    if (client != null)
                    {
                        string result = await paymentService.VerifyTransaction(tokenSession, tokenApp, orderId, cresId ?? "", client);
                        ViewBag.statusPayment = result;
                        ViewBag.showLoading = 'N';
                        ViewBag.orderId = orderId;
                        ViewBag.secuence = secuence;
                    }
                    else
                    {
                        ViewBag.showLoading = 'N';
                        ViewBag.statusPayment = "ERROR";
                        ViewBag.orderId = "";
                        ViewBag.secuence = "";
                    }
                }
                else
                {
                    ViewBag.showLoading = 'N';
                    ViewBag.statusPayment ="ERROR";
                    ViewBag.orderId = "";
                    ViewBag.secuence = "";
                }
                
            }
            catch (Exception ex)
            {
                ViewBag.showLoading = 'N';
                ViewBag.statusPayment = "ERROR";
            }
            return View();
        }

        public IActionResult Links()
        {
            return View();
        }
    }
}

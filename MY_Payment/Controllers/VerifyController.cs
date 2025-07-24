using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MY_Payment.Models;
using MY_Payment.Service;

namespace MY_Payment.Controllers
{
    public class VerifyController : Controller
    {
        private readonly AuthService _authService;
        private readonly ClientService _clientService;
        private readonly IConfiguration _configuration;
        private readonly PaymentService _paymentService;

        public VerifyController(IConfiguration configuration, AuthService authService, PaymentService paymentService, ClientService clientService)
        {
            _configuration = configuration;
            _paymentService = paymentService;
            _clientService = clientService;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                string hostUrl = _configuration.GetValue<string>("globalVariables:hostUrlAdmin")!;
                string hostAppLink = _configuration.GetValue<string>("globalVariables:hostAppLink")!;
                ViewBag.hostUrl = hostUrl;
                ViewBag.hostAppLink =  hostAppLink;
                var queryString = HttpContext.Request.QueryString!.ToString();
                if (!string.IsNullOrEmpty(queryString))
                {
                    string sessionId = HttpContext.Request.Query["ts"].ToString();
                    string orderId = HttpContext.Request.Query["order"].ToString();
                    string cresId = HttpContext.Request.Query["cres"].ToString();
                    string secuence = HttpContext.Request.Query["sec"].ToString();
                    string tokenApp = await _authService.GenerateTokenApplication();
                    string tokenSession = await _authService.GetCurrentTokenSession(sessionId, tokenApp);
                    Client? client = await _clientService.GetClientInfo(tokenSession, tokenApp);
                    if (client != null)
                    {
                        string result = await _paymentService.VerifyTransaction(tokenSession, tokenApp, orderId, cresId ?? "", client);
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

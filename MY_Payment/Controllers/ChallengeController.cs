using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using MY_Payment.Models;
using MY_Payment.Models.Response;
using MY_Payment.Service;

namespace MY_Payment.Controllers
{
    public class ChallengeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        AuthService authService;
        ClientService clientService;
        private IConfiguration configuration;
        PaymentService paymentService;

        public ChallengeController(ILogger<HomeController> logger, IConfiguration _configuration)
        {
            _logger = logger;
            this.configuration = _configuration;
            paymentService = new PaymentService(configuration);
            clientService = new ClientService(configuration);
            authService = new AuthService(configuration);
            ViewBag.showLoading = 'Y';
        }

        public async Task<IActionResult> IndexAsync(string iframe)
        {
            try
            {
                var queryString = HttpContext.Request.QueryString!.ToString();
                if (!string.IsNullOrEmpty(queryString))
                {
                    string sessionId = HttpContext.Request.Query["ts"].ToString();
                    string orderId = HttpContext.Request.Query["order"].ToString();
                    string tokenApp = await authService.GenerateTokenApplication();
                    string tokenSession = await authService.GetCurrentTokenSession(sessionId, tokenApp);
                    Client? client = await clientService.GetClientInfo(tokenSession, tokenApp);
                    if (client != null)
                    {
                        NuveiTransactionFull? orderTransaction = await clientService.GetTransactionByOrderId(tokenSession, tokenApp, orderId)!;
                        ViewBag.iframe = orderTransaction!.browserInfo!.challengeRequest;
                        ViewBag.statusPayment = null;
                    }
                    else
                    {
                        ViewBag.showLoading = 'N';
                        ViewBag.statusPayment = "ERROR";
                    }
                }
                else
                {
                    ViewBag.showLoading = 'N';
                    ViewBag.statusPayment = "ERROR";
                }
            }
            catch (Exception error)
            {
                ViewBag.showLoading = 'N';
                ViewBag.statusPayment = "ERROR";
            }
            return View();
        }

    }
}

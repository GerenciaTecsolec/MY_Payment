using Microsoft.AspNetCore.Mvc;
using MY_Payment.Models;
using MY_Payment.Service;

namespace MY_Payment.Controllers
{
    public class ChallengeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AuthService _authService;
        private readonly ClientService _clientService;

        public ChallengeController(ILogger<HomeController> logger, AuthService authService, ClientService clientService)
        {
            _logger = logger;
            _clientService = clientService;
            _authService = authService;
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
                    string shoppingCartId = HttpContext.Request.Query["cart"].ToString();

                    string tokenApp = await _authService.GenerateTokenApplication();
                    string tokenSession = await _authService.GetCurrentTokenSession(sessionId, tokenApp);
                    Client? client = await _clientService.GetClientInfo(tokenSession, tokenApp);
                    if (client != null)
                    {
                        NuveiTransactionFull? orderTransaction = await _clientService.GetTransactionByShoppingCartId(tokenSession, tokenApp, shoppingCartId)!;
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

using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using MY_Payment.Models;
using MY_Payment.Models.Response;
using MY_Payment.Service;
using System.Diagnostics;
using System.Transactions;

namespace MY_Payment.Controllers
{
    public class HomeController : Controller
    {
        AuthService authService;
        ClientService clientService;
        private IConfiguration configuration;
        PaymentService paymentService;
        CardForm cardForm;

        public HomeController(IConfiguration _configuration)
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
                    string tokenCard = HttpContext.Request.Query["tc"].ToString();
                    string tokenApp = await authService.GenerateTokenApplication();
                    string tokenSession = await authService.GetCurrentTokenSession(sessionId, tokenApp);
                    if (String.IsNullOrEmpty(tokenSession))
                    {
                        ViewBag.showLoading = 'N';
                        ViewBag.card = null;
                        ViewBag.cardError = "SHOW";
                        return View();
                    }
                    Client? client = await clientService.GetClientInfo(tokenSession, tokenApp);
                    if (client != null)
                    {
                        List<ClientCard> clientCardList = await paymentService.GetClientCards(client.id.ToString(), tokenSession, tokenApp);
                        if (clientCardList.Count > 0)
                        {
                            ClientCard card = clientCardList.Find(card => card.token == tokenCard)!;
                            if (card != null)
                            {
                                ClientCard currentCard = card;
                                cardForm = new CardForm()
                                {
                                    holderName = client.name.ToUpper() + " " + client.paternalSurname!.ToUpper(),
                                    number = card.number,
                                    cardBrand = card.cardBrand.brand,
                                    cardLogo = card.cardBrand.logo
                                };
                            }
                        }
                    }
                    ViewBag.cardError = "HIDE";
                    ViewBag.card = cardForm;

                }
                else
                {
                    ViewBag.showLoading = 'N';
                    ViewBag.card = null;
                    ViewBag.cardError = "SHOW";
                }
            }
            catch(Exception error)
            {
                ViewBag.showLoading = 'N';
                ViewBag.card = null;
                ViewBag.cardError = "SHOW";
            }
            return View();
        }

        public async Task<JsonResult> ConfirmPayment(Parameters parameters)
        {
            try
            {
                string tokenCard = parameters.tc ?? "";
                string sessionId = parameters.ts ?? "";
                string clientAddressId = parameters.ad ?? "";
                string currentIp = HttpContext.Connection.RemoteIpAddress!.ToString()!;
                var feature = HttpContext.Features.Get<IHttpConnectionFeature>();
                currentIp = feature!.LocalIpAddress!.ToString();
                string userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                BrowserInfo browserInfo = new BrowserInfo()
                {
                    Ip = "192.168.1.32",//currentIp,
                    Language = "en-US",
                    JavaEnabled = false,
                    JsEnabled = true,
                    ColorDepht = 24,
                    ScreenHeight = 1200,
                    ScreenWidth = 1920,
                    TimezoneOffset = 0,
                    UserAgent = userAgent,
                    AcceptHeader = "text/xml"
                };
                string tokenApp = await authService.GenerateTokenApplication();
                string tokenSession = await authService.GetCurrentTokenSession(sessionId, tokenApp);
                DebitResult? result = await paymentService.GenerateDebit(tokenCard, tokenSession, browserInfo, clientAddressId, tokenApp, sessionId)!;
                return Json(new
                {
                    result!.error,
                    result!.resultCode,
                    result!.codeStatus,
                    result!.iframe,
                    result!.order,
                    result!.transactionId
                });
            }
            catch(Exception error)
            {
                return Json(new
                {
                    error = true,
                    resultCode = "ERROR"
                });
            }
            
        }

        public async Task<JsonResult> VerifyTransaction(Parameters parameters)
        {
            string sessionId = parameters.ts ?? "";
            string orderId = parameters.order!;
            string cresId = parameters!.cresId!;
            string tokenApp = await authService.GenerateTokenApplication();
            string tokenSession = await authService.GetCurrentTokenSession(sessionId, tokenApp);
            Client? client = await clientService.GetClientInfo(tokenSession, tokenApp);
            string paymentStatus = "";
            if (client != null)
            {
                string result = await paymentService.VerifyTransaction(tokenSession, tokenApp, orderId, cresId, client);
                paymentStatus = result;
            }
            ViewBag.showLoading = 'Y';
            return Json(new
            {
                paymentStatus = paymentStatus,
                orderId = orderId,
                secuence = "1213"
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class Parameters
    {
        public string? ts { get; set; }
        public string? tc { get; set; }
        public string? ad { get; set; }
        public string? order { get; set; }
        public string? cresId { get; set; }
    }
}

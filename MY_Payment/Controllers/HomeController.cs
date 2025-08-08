using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using MY_Payment.Models;
using MY_Payment.Models.Response;
using MY_Payment.Service;
using System.Diagnostics;

namespace MY_Payment.Controllers
{
    public class HomeController : Controller
    {
        private readonly AuthService _authService;
        private readonly ClientService _clientService;
        private readonly IConfiguration _configuration;
        private readonly PaymentService _paymentService;
        private readonly ILogger<HomeController> _logger;


        public HomeController(IConfiguration configuration, ILogger<HomeController> logger, AuthService authService, PaymentService paymentService, ClientService clientService)
        {
            _configuration = configuration;
            _logger = logger;
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
                ViewBag.hostAppLink = hostAppLink;
                var queryString = HttpContext.Request.QueryString!.ToString();
                if (!string.IsNullOrEmpty(queryString))
                {
                    CardForm cardForm = new CardForm();
                    string sessionId = HttpContext.Request.Query["ts"].ToString();
                    string tokenCard = HttpContext.Request.Query["tc"].ToString();
                    string tokenApp = await _authService.GenerateTokenApplication();
                    string tokenSession = await _authService.GetCurrentTokenSession(sessionId, tokenApp);
                    if (String.IsNullOrEmpty(tokenSession))
                    {
                        ViewBag.showLoading = 'N';
                        ViewBag.card = null;
                        ViewBag.cardError = "SHOW";
                        return View();
                    }
                    Client? client = await _clientService.GetClientInfo(tokenSession, tokenApp);

                    if(client == null)
                    {
                        _logger.LogError("{event}{message}", "Index", "Client not found.");
                        ViewBag.showLoading = 'N';
                        ViewBag.card = null;
                        ViewBag.cardError = "SHOW";
                        return View();
                    }

                    List<ClientCard> clientCardList = await _paymentService.GetClientCards(client.id.ToString(), tokenSession, tokenApp);
                    if (clientCardList.Count > 0)
                    {
                        ClientCard card = clientCardList.Find(card => card.Token == tokenCard)!;
                        if (card != null)
                        {
                            ClientCard currentCard = card;
                            cardForm = new CardForm()
                            {
                                HolderName = client.name.ToUpper() + " " + client.paternalSurname!.ToUpper(),
                                Number = card.Number.Replace("*", "&bull;"),
                                CardBrand = card.CardBrand.Brand,
                                CardLogo = $"{hostUrl}{card.CardBrand.Logo}"
                            };
                        }
                    }
                    ViewBag.cardError = "HIDE";
                    ViewBag.card = cardForm;
                    return View();
                }
                else
                {
                    _logger.LogError("{event}{message}{other_data}", "Index", "Query string parameters are empty.", queryString);
                    ViewBag.showLoading = 'N';
                    ViewBag.card = null;
                    ViewBag.cardError = "SHOW";
                    return View();
                }
            }
            catch(Exception error)
            {
                _logger.LogCritical("{event}{message}{exception}", "Index", "Error", error);
                ViewBag.showLoading = 'N';
                ViewBag.card = null;
                ViewBag.cardError = "SHOW";
                return View();
            }
            
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
                string tokenApp = await _authService.GenerateTokenApplication();
                string tokenSession = await _authService.GetCurrentTokenSession(sessionId, tokenApp);
                DebitResult? result = await _paymentService.GenerateDebit(tokenCard, tokenSession, browserInfo, clientAddressId, tokenApp, sessionId)!;
                return Json(new
                {
                    result!.error,
                    result!.resultCode,
                    result!.codeStatus,
                    result!.iframe,
                    result!.shoppingCartId,
                    result!.transactionId,
                    result!.secuence
                });
            }
            catch(Exception error)
            {
                _logger.LogCritical("{event}{message}{exception}", "HomeController-ConfirmPayment", "Error", error);
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
            string shoppingCartId = parameters.cart!;
            string cresId = parameters!.cresId!;
            string clientAddressId = parameters!.ad!;
            string tokenApp = await _authService.GenerateTokenApplication();
            string tokenSession = await _authService.GetCurrentTokenSession(sessionId, tokenApp);
            Client? client = await _clientService.GetClientInfo(tokenSession, tokenApp);
            string paymentStatus = "";
            if (client != null)
            {
                string result = await _paymentService.VerifyTransaction(tokenSession, tokenApp, shoppingCartId, cresId, client, clientAddressId, sessionId);
                paymentStatus = result;
            }
            ViewBag.showLoading = 'Y';
            return Json(new
            {
                paymentStatus,
                shoppingCartId,
                secuence = "0"
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
        public string? cart { get; set; }
    }
}

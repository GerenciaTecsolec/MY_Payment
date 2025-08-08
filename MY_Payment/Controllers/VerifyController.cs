using Microsoft.AspNetCore.Mvc;
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
        private readonly ILogger<VerifyController> _logger;

        public VerifyController(IConfiguration configuration, AuthService authService, PaymentService paymentService, ClientService clientService, ILogger<VerifyController> logger)
        {
            _configuration = configuration;
            _paymentService = paymentService;
            _clientService = clientService;
            _authService = authService;
            _logger = logger;
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
                _logger.LogInformation("{event}{message}{other_data}", "Index", "Query string parameters entry.", queryString);
                if (!string.IsNullOrEmpty(queryString))
                {
                    string sessionId = HttpContext.Request.Query["ts"].ToString();
                    string shoppingCartId = HttpContext.Request.Query["cart"].ToString();
                    string cresId = HttpContext.Request.Query["cres"].ToString();
                    string secuence = HttpContext.Request.Query["sec"].ToString();
                    string clientAddressId = HttpContext.Request.Query["ad"].ToString();

                    string tokenApp = await _authService.GenerateTokenApplication();
                    string tokenSession = await _authService.GetCurrentTokenSession(sessionId, tokenApp);
                    
                    Client? client = await _clientService.GetClientInfo(tokenSession, tokenApp);
                    if (client != null)
                    {
                        string result = await _paymentService.VerifyTransaction(tokenSession, tokenApp, shoppingCartId, cresId ?? "", client, clientAddressId, sessionId);
                        ViewBag.statusPayment = result;
                        ViewBag.showLoading = 'N';
                        ViewBag.shoppingCartId = shoppingCartId;
                        ViewBag.secuence = secuence;
                        return View();
                    }
                    else
                    {
                        _logger.LogError("{event}{message}", "Index", "Client not found.");
                        ViewBag.showLoading = 'N';
                        ViewBag.statusPayment = "ERROR";
                        ViewBag.shoppingCartId = "";
                        ViewBag.secuence = "";
                        return View();
                    }
                }
                else
                {
                    _logger.LogError("{event}{message}{other_data}", "Index", "Query string parameters are empty.", queryString);
                    ViewBag.showLoading = 'N';
                    ViewBag.statusPayment ="ERROR";
                    ViewBag.shoppingCartId = "";
                    ViewBag.secuence = "";
                    return View();
                }
            }
            catch (Exception error)
            {
                _logger.LogCritical("{event}{message}{exception}", "Index", "Error", error);
                ViewBag.showLoading = 'N';
                ViewBag.statusPayment = "ERROR";
                ViewBag.shoppingCartId = "";
                ViewBag.secuence = "";
                return View();
            }
        }

        public IActionResult Links()
        {
            return View();
        }
    }
}

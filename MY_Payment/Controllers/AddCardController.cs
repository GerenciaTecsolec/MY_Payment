using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MY_Payment.Models;
using MY_Payment.Service;

namespace MY_Payment.Controllers
{
    public class AddCardController : Controller
    {
        private readonly AuthService _authService;
        private readonly ClientService _clientService;
        private readonly IConfiguration _configuration;

        public AddCardController(IConfiguration configuration, AuthService authService, ClientService clientService)
        {
            _configuration = configuration;
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
                    string sessionId = HttpContext.Request.Query["ts"].ToString();
                    string tokenApp = await _authService.GenerateTokenApplication();
                    string tokenSession = await _authService.GetCurrentTokenSession(sessionId, tokenApp);
                    if (String.IsNullOrEmpty(tokenSession))
                    {
                        ViewBag.showLoading = 'N';
                        ViewBag.cardError = "SHOW";
                        return View();
                    }
                    Client? client = await _clientService.GetClientInfo(tokenSession, tokenApp);
                    if (client != null)
                    {
                        string enviroment = _configuration.GetValue<string>("globalVariables:enviroment")!;
                        ViewBag.enviroment = "stg";
                        if (enviroment.Equals("PROD"))
                        {
                            ViewBag.enviroment = "prod";
                        }

                        string tag = "";
                        switch (enviroment)
                        {
                            case "QA":
                                tag = "Nuvei:add-card:";
                                break;
                            case "PROD":
                                tag = "Nuvei:production:";
                                break;
                            default:
                                tag = "Nuvei:debug:";
                                break;
                        }

                        string server_application_code = _configuration.GetValue<string>(tag + "clientAppCode")!;
                        string server_app_key = _configuration.GetValue<string>(tag + "clientAppKey")!;
                        ViewBag.appCode = server_application_code;
                        ViewBag.appKey = server_app_key;
                        ViewBag.showLoading = 'N';
                        ViewBag.cardError = "HIDE";
                        ViewBag.email = client!.email!;
                        ViewBag.uid = client!.id!.ToString();
                        return View();
                    }
                    ViewBag.showLoading = 'N';
                    ViewBag.cardError = "SHOW";
                    return View();
                }
                else
                {
                    ViewBag.showLoading = 'N';
                    ViewBag.cardError = "SHOW";
                   return View();
                }
            }
            catch (Exception error)
            {
                ViewBag.showLoading = 'N';
                ViewBag.cardError = "SHOW";
            }
            return View();
        }
    }
}

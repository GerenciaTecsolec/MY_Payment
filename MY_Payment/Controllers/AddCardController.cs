using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MY_Payment.Models;
using MY_Payment.Service;

namespace MY_Payment.Controllers
{
    public class AddCardController : Controller
    {
        AuthService authService;
        ClientService clientService;
        private IConfiguration configuration;

        public AddCardController(IConfiguration _configuration)
        {
            this.configuration = _configuration;
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
                    string tokenApp = await authService.GenerateTokenApplication();
                    string tokenSession = await authService.GetCurrentTokenSession(sessionId, tokenApp);
                    if (String.IsNullOrEmpty(tokenSession))
                    {
                        ViewBag.showLoading = 'N';
                        ViewBag.cardError = "SHOW";
                        return View();
                    }
                    Client? client = await clientService.GetClientInfo(tokenSession, tokenApp);
                    if (client != null)
                    {
                        string enviroment = configuration.GetValue<string>("globalVariables:enviroment")!;
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

                        string server_application_code = configuration.GetValue<string>(tag + "clientAppCode")!;
                        string server_app_key = configuration.GetValue<string>(tag + "clientAppKey")!;
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

using MY_Payment.Models;
using MY_Payment.Models.Request;
using MY_Payment.Models.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace MY_Payment.Service
{
    public class ClientService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ClientService> _logger;

        public ClientService(IConfiguration configuration, ILogger<ClientService> logger)
        {
           _configuration = configuration;
            _logger = logger;
        }
        
        public async Task<Client?> GetClientInfo(string tokenSession, string tokenApp)
        {
            string url = string.Empty;
            using (var client = new HttpClient())
            {
                try
                {
                    url = _configuration.GetValue<string>("globalVariables:hostUrl")!;
                    var path = "/api/Client";
                    client.CancelPendingRequests();
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(25);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("tokenSession", tokenSession);
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenApp);
                    var response = await client.GetAsync(url + path);
                    if (response.IsSuccessStatusCode)
                    {
                        string readTask = await response.Content.ReadAsStringAsync();
                        ClientInfoResponse result = JsonConvert.DeserializeObject<ClientInfoResponse>(readTask, new JsonSerializerSettings { Error = (sender, error) => error.ErrorContext.Handled = true });
                        return result!.result;
                    }
                    else
                    {
                        string readTask = await response.Content.ReadAsStringAsync();
                        _logger.LogError("{event}{message}{other_data}", "ClientService-GetClientInfo", "Error get ClientInfo", readTask);
                        return null;
                    }
                }
                catch (Exception error)
                {
                    _logger.LogCritical("{event}{message}{exception}", "ClientService-GetClientInfo", "Error", error);
                    throw;
                }
            }
        }

        public async Task<OrderMY?> GetOrderById(string tokenSession, string tokenApp, string orderId)
        {
            string url = string.Empty;
            using (var client = new HttpClient())
            {
                try
                {
                    url = _configuration.GetValue<string>("globalVariables:hostUrl")!;
                    var path = "/api/Order/id?orderId=" + orderId;
                    client.CancelPendingRequests();
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(25);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("tokenSession", tokenSession);
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenApp);
                    var response = await client.GetAsync(url + path);
                    if (response.IsSuccessStatusCode)
                    {
                        string readTask = await response.Content.ReadAsStringAsync();
                        OrderByIdResponse orderBYIdResponse = JsonConvert.DeserializeObject<OrderByIdResponse>(readTask, new JsonSerializerSettings { Error = (sender, error) => error.ErrorContext.Handled = true })!;
                        OrderMY result = orderBYIdResponse!.result;
                        return result!;
                    }
                    else
                    {
                        string readTask = await response.Content.ReadAsStringAsync();
                        _logger.LogError("{event}{message}{other_data}", "ClientService-GetOrderById", "Error get order by Id", readTask);
                        return null;
                    }
                }
                catch (Exception error)
                {
                    _logger.LogCritical("{event}{message}{exception}", "ClientService-GetOrderById", "Error", error);
                    throw;
                }
            }
        }


        public async Task<string?> GetCurrentShoppingCart(string tokenSession, string tokenApp)
        {
            var baseUrl = _configuration.GetValue<string>("globalVariables:hostUrl");

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                _logger.LogError("{event}{message}", "GetCurrentShoppingCart", "Missing or invalid hostUrl configuration.");
                return null;
            }

            var requestUri = $"{baseUrl}/api/ShoppingCart/client";

            try
            {
                using var client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(25)
                };

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("tokenSession", tokenSession);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenApp);
                
                var response = await client.GetAsync(requestUri);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var jObject = JObject.Parse(responseContent);
                    var shoppingCartId = jObject["result"]?["id"]?.ToString();
                    return shoppingCartId;
                }

                _logger.LogError("{event}{message}{response}", "GetCurrentShoppingCart", "Error getting client shoppingCart.", responseContent);
                return null;
            } 
            catch(Exception exception)
            {
                _logger.LogCritical("{event}{message}{exception}", "GetCurrentShoppingCart", "Error", exception);
                throw;
            }
        }

        public async Task<NuveiTransactionFull?> GetTransactionByShoppingCartId(string tokenSession, string tokenApp, string shoppingCartId, string sessionId)
        {
            string url = string.Empty;
            using (var client = new HttpClient())
            {
                try
                {
                    url = _configuration.GetValue<string>("globalVariables:hostUrl")!;
                    var path = $"/api/ShoppingCart/id/transaction?shoppingCartId={shoppingCartId}&session={sessionId}";
                    client.CancelPendingRequests();
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(25);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("tokenSession", tokenSession);
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenApp);
                    var response = await client.GetAsync(url + path);
                    if (response.IsSuccessStatusCode)
                    {
                        string readTask = await response.Content.ReadAsStringAsync();
                        TransactionByOrderResponse result = JsonConvert.DeserializeObject<TransactionByOrderResponse>(readTask, new JsonSerializerSettings { Error = (sender, error) => error.ErrorContext.Handled = true })!;
                        return result!.result;
                    }
                    else
                    {
                        string readTask = await response.Content.ReadAsStringAsync();
                        _logger.LogError("{event}{message}{other_data}", "GetTransactionByShoppingCartId", "Error get order transaction by shoppingCartId", readTask);
                        return null;
                    }
                }
                catch (Exception error)
                {
                    _logger.LogCritical("{event}{message}{exception}", "GetTransactionByShoppingCartId", "Error", error);
                    throw;
                }
            }
        }

        public async Task CreateInvoice(string tokenSession, string tokenApp, string orderId)
        {
            string url = string.Empty;
            using (var client = new HttpClient())
            {
                try
                {
                    url = _configuration.GetValue<string>("globalVariables:hostUrl")!;
                    CreateInvoicePayload authPayload = new CreateInvoicePayload()
                    {
                        orderId = orderId,
                    };
                    var path = "/api/Invoice/create";
                    client.CancelPendingRequests();
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(25);
                    var payload = JsonConvert.SerializeObject(authPayload);
                    var content = new StringContent(payload, Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Add("tokenSession", tokenSession);
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenApp);
                    HttpResponseMessage response = await client.PostAsync(url + path, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = await response.Content.ReadAsStringAsync();
                        GetTokenResponse result = JsonConvert.DeserializeObject<GetTokenResponse>(readTask, new JsonSerializerSettings { Error = (sender, error) => error.ErrorContext.Handled = true })!;
                        _logger.LogInformation("{event}{message}{other_data}", "CreateInvoice", "Create Invoice response", result);
                    }
                    else
                    {
                        string readTask = await response.Content.ReadAsStringAsync();
                        _logger.LogError("{event}{message}{other_data}", "CreateInvoice", "Error to create invoice", readTask);
                    }
                }
                catch (Exception error)
                {
                    _logger.LogCritical("{event}{message}{exception}", "CreateInvoice", "Error", error);
                    throw;
                }

            }
        }

    }
}

using MY_Payment.Models;
using MY_Payment.Models.Request;
using MY_Payment.Models.Response;
using Newtonsoft.Json;
using System.Text;

namespace MY_Payment.Service
{
    public class ClientService
    {
        private readonly IConfiguration configuration;

        public ClientService(IConfiguration _configuration)
        {
            this.configuration = _configuration;
        }
        
        public async Task<Client?> GetClientInfo(string tokenSession, string tokenApp)
        {
            string url = "";
            using (var client = new HttpClient())
            {
                try
                {
                    url = configuration.GetValue<string>("globalVariables:hostUrl")!;
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
                        return null;
                    }
                }
                catch (Exception error)
                {
                    throw;
                }
            }
        }

        public async Task<OrderMY?> GetOrderById(string tokenSession, string tokenApp, string orderId)
        {
            string url = "";
            using (var client = new HttpClient())
            {
                try
                {
                    url = configuration.GetValue<string>("globalVariables:hostUrl")!;
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
                        return null;
                    }
                }
                catch (Exception error)
                {
                    throw;
                }
            }
        }

        public async Task<NuveiTransactionFull> GetTransactionByOrderId(string tokenSession, string tokenApp, string orderId)
        {
            string url = "";
            using (var client = new HttpClient())
            {
                try
                {
                    url = configuration.GetValue<string>("globalVariables:hostUrl")!;
                    var path = "/api/Order/id/transaction?orderId=" + orderId;
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
                        return null;
                    }
                }
                catch (Exception error)
                {
                    throw;
                }
            }
        }

        public async Task<string> CreateInvoice(string tokenSession, string tokenApp, string orderId)
        {
            string url = "";
            using (var client = new HttpClient())
            {
                try
                {
                    url = configuration.GetValue<string>("globalVariables:hostUrl")!;
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
                        if (result!.result)
                        {
                            return "OK";
                        }
                        return "";
                    }
                    else
                    {
                        return "";
                    }
                }
                catch (Exception error)
                {
                    throw;
                }

            }
        }

    }
}

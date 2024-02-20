using Microsoft.Extensions.Configuration;
using MY_Payment.Models;
using MY_Payment.Models.Response;
using Newtonsoft.Json;
using System.Text;

namespace MY_Payment.Service
{
    public class AuthService
    {
        private readonly IConfiguration configuration;
        public AuthService(IConfiguration _configuration)
        {
            this.configuration = _configuration;
        }

        public async Task<string> GenerateTokenApplication()
        {
            string url = "";
            using (var client = new HttpClient())
            {
                try
                {
                    url = configuration.GetValue<string>("globalVariables:hostUrl")!;
                    AuthPayload authPayload = new AuthPayload()
                    {
                        clientId = "OaMcdkAEFCqGbDUdtjOY",
                        clientSecret = "OJtqiFGzNgvAqgwYVMijyUCTknWZwjOGQmATroZf"
                    };
                    var path = "/api/Security/get-token";
                    client.CancelPendingRequests();
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(25);
                    var payload = JsonConvert.SerializeObject(authPayload);
                    var content = new StringContent(payload, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(url + path, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = await response.Content.ReadAsStringAsync();
                        GetTokenResponse result = JsonConvert.DeserializeObject<GetTokenResponse>(readTask, new JsonSerializerSettings { Error = (sender, error) => error.ErrorContext.Handled = true })!;
                        if (result!.result) {
                            return result!.token;
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

        public async Task<string> GetCurrentTokenSession(string sessionId, string tokenApp)
        {
            string url = "";
            using (var client = new HttpClient())
            {
                try
                {
                    url = configuration.GetValue<string>("globalVariables:hostUrl")!;
                    var path = "/api/Security/session?sessionId=" + sessionId;
                    client.CancelPendingRequests();
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenApp);
                    var response = await client.GetAsync(url + path);
                    if (response.IsSuccessStatusCode)
                    {
                        string readTask = await response.Content.ReadAsStringAsync();
                        GetTokenResponse result = JsonConvert.DeserializeObject<GetTokenResponse>(readTask, new JsonSerializerSettings { Error = (sender, error) => error.ErrorContext.Handled = true })!;
                        if (result!.result)
                        {
                            return result!.token;
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

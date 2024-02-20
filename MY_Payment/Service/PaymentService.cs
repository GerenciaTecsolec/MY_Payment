using MY_Payment.Models;
using Newtonsoft.Json;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Dynamic;
using MY_Payment.Models.Response;
using System.Globalization;
using MY_Payment.Models.Request;

namespace MY_Payment.Service
{
    public class PaymentService
    {
        private readonly IConfiguration configuration;
        AuthService authService;
        ClientService clientService;

        public PaymentService(IConfiguration _configuration)
        {
            this.configuration = _configuration;
            this.clientService = new ClientService(configuration);
            this.authService = new AuthService(configuration);
        }

        private string GenerateToken(string credentialType)
        {
            try
            {
                string server_application_code = "";
                string server_app_key = "";
                string enviroment = configuration.GetValue<string>("globalVariables:enviroment")!;
                string tag = "";
                switch (enviroment)
                {
                    case "QA":
                        tag = "Nuvei:qa:";
                        break;
                    case "PROD":
                        tag = "Nuvei:production:";
                        break;
                    default:
                        tag = "Nuvei:debug:";
                        break;
                }
                switch (credentialType)
                {
                    case "SERVER":
                        server_application_code = configuration.GetValue<string>(tag + "serverAppCode") ?? "";
                        server_app_key = configuration.GetValue<string>(tag + "serverAppKey") ?? "";
                        break;
                    case "CLIENT":
                        server_application_code = configuration.GetValue<string>(tag + "clientAppCode") ?? "";
                        server_app_key = configuration.GetValue<string>(tag + "clientAppKey") ?? "";
                        break;
                    default:
                        break;
                }


                DateTime date = DateTime.UtcNow;
                long unix_timestamp = (long)(date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                string uniq_token_string = server_app_key + unix_timestamp;
                byte[] uniq_token_hash_bytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(uniq_token_string));
                string uniq_token_hash = BitConverter.ToString(uniq_token_hash_bytes).Replace("-", "").ToLower();
                string fullToken_hash = $"{server_application_code};{unix_timestamp};{uniq_token_hash}";
                string auth_token = Convert.ToBase64String(Encoding.UTF8.GetBytes(fullToken_hash));
                return auth_token;
            }
            catch (Exception error)
            {
                throw;
            }
        }

        public async Task<List<CardBrand>> GetCardBrandList(string tokenSession, string tokenApp)
        {
            string url = "";
            using (var client = new HttpClient())
            {
                try
                {
                    url = configuration.GetValue<string>("globalVariables:hostUrl")!;
                    var path = "/api/Payment/card-brand";
                    client.CancelPendingRequests();
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("tokenSession", tokenSession);
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenApp);

                    var response = await client.GetAsync(url + path);
                    if (response.IsSuccessStatusCode)
                    {
                        string readTask = await response.Content.ReadAsStringAsync();
                        CardBrandListResponse result = JsonConvert.DeserializeObject<CardBrandListResponse>(readTask)!;
                        return result!.result;
                    }
                    else
                    {
                        return new List<CardBrand>();
                    }
                }
                catch (Exception error)
                {
                    throw;
                }
            }
        }

        public async Task<List<ClientCard>> GetClientCards(string clientId, string tokenSession, string tokenApp)
        {
            string url = "";
            string enviroment = configuration.GetValue<string>("globalVariables:enviroment")!;
            string tag = "";
            switch (enviroment)
            {
                case "QA":
                    tag = "Nuvei:qa:";
                    break;
                case "PROD":
                    tag = "Nuvei:production:";
                    break;
                default:
                    tag = "Nuvei:debug:";
                    break;
            }
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {
                try
                {
                    url = configuration.GetValue<string>(tag + "url") ?? "";
                    var path = string.Format("/v2/card/list?uid={0}", clientId);
                    client.CancelPendingRequests();
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    string token = GenerateToken("SERVER");
                    client.DefaultRequestHeaders.Add("Auth-Token", token);
                    var response = await client.GetAsync(url + path);
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<CardListResponse>(readTask);
                        if (result!.cards!.Count > 0)
                        {
                            List<CardBrand> cardBrandList = await GetCardBrandList(tokenSession, tokenApp);
                            List<ClientCard> clientCards = new List<ClientCard>();
                            foreach (var card in result.cards)
                            {
                                var brand = cardBrandList.Find(x => x.cardType.ToString().ToUpper() == card?.type?.ToUpper());
                                ClientCard clientCard = new ClientCard()
                                {
                                    holderName = card!.holderName!.ToUpper(),
                                    number = card!.bin + " *** *** " + card!.number,
                                    token = card!.token ?? "",
                                    cardBrand = brand!
                                };
                                clientCards.Add(clientCard);
                            }

                            return clientCards;
                        }

                        return new List<ClientCard>();
                    }
                    else
                    {
                        return new List<ClientCard>();
                    }
                }
                catch (Exception error)
                {
                    throw;
                }
            }
        }

        public async Task<DebitResult?> GenerateDebit(string tokenCard, string tokenSession, BrowserInfo browserInfo, string clientAddressId, string tokenApp, string sessionId)
        {
            string url = "";
             DebitResult debit = new DebitResult();
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {
                try
                {
                    Client? currentClient = await clientService.GetClientInfo(tokenSession, tokenApp);
                    if (currentClient == null)
                    {
                        throw new("No se encuentra información del cliente.");
                    }

                    var order = await this.ConfirmOrder(clientAddressId, tokenCard, tokenSession);
                    if (order!.Order == null)
                    {
                        throw new("Error al confirmar la orden.");
                    }

                    OrderMY? currentOrder = await this.clientService.GetOrderById(tokenSession, tokenApp, order!.Order!.Id!.ToString()!)!;
                    if (currentOrder == null)
                    {
                        throw new("Orden no existe.");
                    }

                    string enviroment = configuration.GetValue<string>("globalVariables:enviroment")!;
                    string tag = "";
                    switch (enviroment)
                    {
                        case "QA":
                            tag = "Nuvei:qa:";
                            break;
                        case "PROD":
                            tag = "Nuvei:production:";
                            break;
                        default:
                            tag = "Nuvei:debug:";
                            break;
                    }

                    url = configuration.GetValue<string>(tag + "url") ?? "";
                    var user = new NuveiDebitUser();
                    user.id = currentClient!.id!.ToString();
                    user.email = currentClient!.email!;

                    string percentageServiceTax = configuration.GetValue<string>("globalVariables:service") ?? "0";
                    decimal serviceTax = (Convert.ToDecimal(percentageServiceTax) / (decimal)100);

                    var debitOrder = new NuveiDebitOrder();
                    debitOrder.Amount = currentOrder!.total!;
                    debitOrder.Description = "Mercado YA, pago de la orden N° 12345";
                    debitOrder.DevReference = order!.Order!.Id!.ToString()!;
                    debitOrder.Vat = 0;
                    debitOrder.TaxPercentage = 0;
                    debitOrder.TaxableAmount = 0;

                    var card = new NuveiDebitCard();
                    card.token = tokenCard;

                    ThreeDs2Data threeDs2Data = new ThreeDs2Data();
                    threeDs2Data.ProcessAnyway = false;
                    threeDs2Data.DeviceType = "browser";

                    string webhook = configuration.GetValue<string>("globalVariables:webhook")!;

                    threeDs2Data.TermUrl = webhook + "/" + order!.Order!.Id!.ToString()! + "/" + sessionId + "/" + order!.Order!.Number!;

                    ExtraParams extraParams = new ExtraParams();
                    extraParams.BrowserInfoData = browserInfo;
                    extraParams.threeDs2Data = threeDs2Data;

                    var request = new NuveiDebitRequest();
                    request.user = user;
                    request.order = debitOrder;
                    request.card = card;
                    request.extra_params = extraParams;

                    var path = "/v2/transaction/debit";
                    client.CancelPendingRequests();
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    var payload = JsonConvert.SerializeObject(request, new DecimalFormatConverter());
                    var content = new StringContent(payload, Encoding.UTF8, "application/json");
                    string token = GenerateToken("SERVER");
                    client.DefaultRequestHeaders.Add("Auth-Token", token);
                    HttpResponseMessage response = await client.PostAsync(url + path, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = await response.Content.ReadAsStringAsync();
                        NuveiDebitWithTokenResponse result = JsonConvert.DeserializeObject<NuveiDebitWithTokenResponse>(readTask, new JsonSerializerSettings { Error = (sender, error) => error.ErrorContext.Handled = true })!;
                        await this.SaveDebitTransaction(result!, order!.Order!.Id!.ToString()!);
                        string iframe = "";
                        if (result != null)
                        {
                            int statusDetail = result!.Transaction!.StatusDetail!;
                            if (statusDetail == 35)
                            {
                                debit = new DebitResult()
                                {
                                    error = false,
                                    codeStatus = statusDetail!.ToString(),
                                    iframe = result!.ThreeDs!.BrowserResponse!.HiddenIframe!,
                                    order = order!.Order!.Id!.ToString()!
                                };
                            }
                            if (statusDetail == 36)
                            {
                                debit = new DebitResult()
                                {
                                    error = false,
                                    codeStatus = statusDetail!.ToString(),
                                    iframe = result!.ThreeDs!.BrowserResponse!.ChallengeRequest!,
                                    transactionId = result!.Transaction!.Id!.ToString()!,
                                    order = order!.Order!.Id!.ToString()!
                                };
                            }
                            if (statusDetail == 31)
                            {
                                debit = new DebitResult()
                                {
                                    error = false,
                                    codeStatus = statusDetail!.ToString(),
                                    order = order!.Order!.Id!.ToString()!
                                };
                            }
                            if (statusDetail == 39)
                            {
                                debit = new DebitResult()
                                {
                                    error = false,
                                    codeStatus = statusDetail!.ToString(),
                                    iframe = result!.ThreeDs!.BrowserResponse!.ChallengeRequest!,
                                    transactionId = result!.Transaction!.Id!.ToString()!,
                                    order = order!.Order!.Id!.ToString()!
                                };
                            }
                        }
                        return debit;
                    }
                    else
                    {
                        var readTask = await response.Content.ReadAsStringAsync();
                        debit = new DebitResult()
                        {
                            error = true,
                            resultCode = "ERROR",
                            codeStatus = "0",
                            iframe = "",
                            order = ""
                        };
                        return debit;
                    }
                }
                catch (Exception error)
                {
                    throw;
                }
            }
        }

        public async Task<string> SaveDebitTransaction(NuveiDebitWithTokenResponse data, string orderId)
        {
            string url = "";
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {
                try
                {
                    string tokenApp = await authService.GenerateTokenApplication();
                    url = configuration.GetValue<string>("globalVariables:hostUrl")!;
                    var path = "/api/Payment/debit-status/order/" + orderId;
                    client.CancelPendingRequests();
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    var payload = JsonConvert.SerializeObject(data);
                    var content = new StringContent(payload, Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenApp);
                    HttpResponseMessage response = await client.PostAsync(url + path, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = await response.Content.ReadAsStringAsync();
                        return "OK";
                    }
                    else
                    {
                        var readTask = await response.Content.ReadAsStringAsync();
                        return "Error al registrar el resultado del debito";
                    }
                }
                catch (Exception error)
                {
                    throw;
                }
            }
        }

        public async Task<ConfirmOrderResponse> ConfirmOrder(string clientAddressId, string tokenCard, string tokenSession)
        {
            string url = "";
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {
                try
                {
                    dynamic request = new ExpandoObject();
                    request.token = tokenCard;
                    request.clientAddressId = clientAddressId;

                    string tokenApp = await authService.GenerateTokenApplication();
                    url = configuration.GetValue<string>("globalVariables:hostUrl")!;
                    var path = "/api/Payment/confirm";
                    client.CancelPendingRequests();
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    var payload = JsonConvert.SerializeObject(request);
                    var content = new StringContent(payload, Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenApp);
                    client.DefaultRequestHeaders.Add("tokenSession", tokenSession);
                    HttpResponseMessage response = await client.PostAsync(url + path, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = await response.Content.ReadAsStringAsync();
                        ConfirmOrderResponse result = JsonConvert.DeserializeObject<ConfirmOrderResponse>(readTask, new JsonSerializerSettings { Error = (sender, error) => error.ErrorContext.Handled = true })!;
                        return result;
                    }
                    else
                    {
                        var readTask = await response.Content.ReadAsStringAsync();
                        ConfirmOrderResponse result = JsonConvert.DeserializeObject<ConfirmOrderResponse>(readTask, new JsonSerializerSettings { Error = (sender, error) => error.ErrorContext.Handled = true })!;
                        return result;
                    }
                }
                catch (Exception error)
                {
                    throw;
                }
            }
        }


        public async Task<string> VerifyTransaction(string tokenSession, string tokenApp, string orderId, string? cres, Client currentClient)
        {
            try
            {
                string hostUrl = configuration.GetValue<string>("globalVariables:hostUrl")!;

                OrderMY? order = await this.clientService.GetOrderById(tokenSession, tokenApp, orderId)!;
                if (order == null)
                {
                    throw new("Orden no existe.");
                }

                NuveiTransactionFull? orderTransactionFull = await clientService.GetTransactionByOrderId(tokenSession, tokenApp, orderId)!;
                NuveiTransaction? orderTransaction = orderTransactionFull!.transaction;
                if (orderTransaction == null)
                {
                    throw new("No existe la transaccion de la orden.");
                }

                string enviroment = configuration.GetValue<string>("globalVariables:enviroment")!;
                string tag = "";
                switch (enviroment)
                {
                    case "QA":
                        tag = "Nuvei:qa:";
                        break;
                    case "PROD":
                        tag = "Nuvei:production:";
                        break;
                    default:
                        tag = "Nuvei:debug:";
                        break;
                }

                string url = configuration.GetValue<string>(tag + "url") ?? "";
                using (var client = new HttpClient())
                {
                    dynamic user = new ExpandoObject();
                    user.id = currentClient!.id!.ToString();

                    dynamic transaction = new ExpandoObject();
                    transaction.id = orderTransaction!.nuveiTransactionId;

                    dynamic request = new ExpandoObject();
                    request.user = user;
                    request.transaction = transaction;
                    request.type = GetVerifyType(orderTransaction!.statusDetail ?? 0);
                    request.value = cres!;
                    request.more_info = true;

                    var path = "/v2/transaction/verify";
                    client.CancelPendingRequests();
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(25);
                    var payload = JsonConvert.SerializeObject(request);
                    var content = new StringContent(payload, Encoding.UTF8, "application/json");
                    string token = GenerateToken("SERVER");
                    client.DefaultRequestHeaders.Add("Auth-Token", token);
                    HttpResponseMessage response = await client.PostAsync(url + path, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = await response.Content.ReadAsStringAsync();
                        NuveiDebitWithTokenResponse result = JsonConvert.DeserializeObject<NuveiDebitWithTokenResponse>(readTask, new JsonSerializerSettings { Error = (sender, error) => error.ErrorContext.Handled = true })!;
                        await this.SaveDebitTransaction(result!, order!.id.ToString()!);
                        if (result!.Transaction!.StatusDetail == 3)
                        {
                            //Aqui se envia email de la confirmacion del debito y la confirmacionde la orden al chef
                            string emailContent = DebitNotificationEmail(orderTransaction!.nuveiTransactionId!, result!.Transaction!.AuthorizationCode!, "Pago de la orden N° " + order!.secuence! + ".", (order!.amount + order!.deliveryFee));
                            EmailRequest emailRequest = new EmailRequest()
                            {
                                sendTo = currentClient!.email!,
                                copyTo = "",
                                content = emailContent,
                                subject = "Autorizacion de Compra MercadoYa - Paymentez"
                            };
                            await SendEmail(emailRequest);
                            await ConfirmPaymentOrder(order!.id.ToString(), tokenSession);
                            clientService.CreateInvoice(tokenSession, tokenApp, order!.id.ToString());

                            return "SUCCESS";
                        }
                        else
                        {
                            return "FAILED";
                        }
                    }
                    else
                    {
                        var readTask = await response.Content.ReadAsStringAsync();
                        return "ERROR";
                    }
                }

            }
            catch (Exception error)
            {
                return "ERROR";
            }
        }


        public string GetVerifyType(int code)
        {
            string verifyType = "";
            switch (code)
            {
                case 35:
                    verifyType = "AUTHENTICATION_CONTINUE";
                    break;
                case 36:
                    verifyType = "BY_CRES";
                    break;
                case 31:
                    verifyType = "BY_OTP";
                    break;
            }

            return verifyType;
        }

        public string DebitNotificationEmail(string idTransaction, string authorizationCode, string detail, decimal amount)
        {
            string content = String.Format(@"<div style=""display:grid;width:600px;margin:0px auto;border:3px solid #d1d1d1;border-radius: 20px;background-color: #FFFFFF;"">
                                                <div style=""background-color: #1aa182 !important;border-top-right-radius: 17px;border-top-left-radius: 17px;width:100%"">
                                                    <img src=""https://www.tecsolec.com/chef/images/background/background-dos.png"" height=""100"" style=""margin: 0px 30%;""/>
                                                </div>
                                                <p style=""text-indent: 0pt;line-height: 0pt;text-align: center;"">
                                                  <span
                                                    style=""color: black; font-family:Verdana, sans-serif; font-style: normal; font-weight: 600; text-decoration: none; font-size: 12pt;"">
                                                    AUTORIZACION DE COMPRA
                                                  </span>
                                                </p>
                                                <div style=""background-color: #19a182;width: 400px; height:62px;margin-bottom: 24px;margin-top: 24px;margin-left:100px;"">
                                                  <div style=""background-color: #FFFFFF;width: 390px; height:61px;"">
                                                    <div style=""padding: 4px;"">
                                                      <p style=""text-indent: 0pt;line-height: 0pt;text-align: left;"">
                                                        <span
                                                          style=""color: black; font-family:Verdana, sans-serif; font-style: normal; font-weight: 600; text-decoration: none; font-size: 12pt;"">
                                                          ID Transacci&oacute;n
                                                        </span>
                                                      </p>
                                                      <p style=""padding-top: 8pt;text-indent: 0pt;line-height: 87%;text-align: left;"">
                                                        <span
                                                          style=""color: #323131;font-style: normal; font-weight: normal; text-decoration: none; font-size: 12pt;"">
                                                          {0}
                                                        </span>
                                                      </p>
                                                    </div>
                                                  </div>
                                                </div>
                                                <div style=""background-color: #19a182;width: 400px; height:62px;margin-bottom: 24px;margin-left:100px;"">
                                                  <div style=""background-color: #FFFFFF;width: 390px; height:61px;"">
                                                    <div style=""padding: 4px;"">
                                                      <p style=""text-indent: 0pt;line-height: 0pt;text-align: left;"">
                                                        <span
                                                          style=""color: black; font-family:Verdana, sans-serif; font-style: normal; font-weight: 600; text-decoration: none; font-size: 12pt;"">
                                                          C&oacute;digo de autorizaci&oacute;n
                                                        </span>
                                                      </p>
                                                      <p style=""padding-top: 8pt;text-indent: 0pt;line-height: 87%;text-align: left;"">
                                                        <span
                                                          style=""color: #323131;font-style: normal; font-weight: normal; text-decoration: none; font-size: 12pt;"">
                                                          {1}
                                                        </span>
                                                      </p>
                                                    </div>
                                                  </div>
                                                </div>
                                                <div style=""background-color: #19a182;width: 400px; height:62px;margin-bottom: 24px;margin-left:100px;"">
                                                  <div style=""background-color: #FFFFFF;width: 390px; height:61px;"">
                                                    <div style=""padding: 4px;"">
                                                      <p style=""text-indent: 0pt;line-height: 0pt;text-align: left;"">
                                                        <span
                                                          style=""color: black; font-family:Verdana, sans-serif; font-style: normal; font-weight: 600; text-decoration: none; font-size: 12pt;"">
                                                          Monto
                                                        </span>
                                                      </p>
                                                      <p style=""padding-top: 8pt;text-indent: 0pt;line-height: 87%;text-align: left;"">
                                                        <span
                                                          style=""color: #323131;font-style: normal; font-weight: normal; text-decoration: none; font-size: 12pt;"">
                                                          {2}
                                                        </span>
                                                      </p>
                                                    </div>
                                                  </div>
                                                </div>
                                                <div style=""background-color: #19a182;width: 400px; height:62px;margin-bottom: 24px;margin-left:100px;"">
                                                  <div style=""background-color: #FFFFFF;width: 390px; height:61px;"">
                                                    <div style=""padding: 4px;"">
                                                      <p style=""text-indent: 0pt;line-height: 0pt;text-align: left;"">
                                                        <span
                                                          style=""color: black; font-family:Verdana, sans-serif; font-style: normal; font-weight: 600; text-decoration: none; font-size: 12pt;"">
                                                          Concepto
                                                        </span>
                                                      </p>
                                                      <p style=""padding-top: 8pt;text-indent: 0pt;line-height: 87%;text-align: left;"">
                                                        <span
                                                          style=""color: #323131;font-style: normal; font-weight: normal; text-decoration: none; font-size: 12pt;"">
                                                          {3}
                                                        </span>
                                                      </p>
                                                    </div>
                                                  </div>
                                                </div>
                                                <div style=""background-color: #19a182;width: 400px; height:62px;margin-bottom: 24px;margin-left:100px;"">
                                                  <div style=""background-color: #FFFFFF;width: 390px; height:61px;"">
                                                    <div style=""padding: 4px;"">
                                                      <p style=""text-indent: 0pt;line-height: 0pt;text-align: left;"">
                                                        <span
                                                          style=""color: black; font-family:Verdana, sans-serif; font-style: normal; font-weight: 600; text-decoration: none; font-size: 12pt;"">
                                                          Fecha de transacci&oacute;n
                                                        </span>
                                                      </p>
                                                      <p style=""padding-top: 8pt;text-indent: 0pt;line-height: 87%;text-align: left;"">
                                                        <span
                                                          style=""color: #323131;font-style: normal; font-weight: normal; text-decoration: none; font-size: 12pt;"">
                                                          {4}
                                                        </span>
                                                      </p>
                                                    </div>
                                                  </div>
                                                </div>
                                              </div>", idTransaction, authorizationCode, amount.ToString(), detail, DateTime.UtcNow.ToLongDateString() + " " + DateTime.UtcNow.ToShortTimeString());
            return content;
        }

        public async Task<string> SendEmail(EmailRequest request)
        {
            string url = "";
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {
                try
                {
                    string tokenApp = await authService.GenerateTokenApplication();
                    url = configuration.GetValue<string>("globalVariables:hostUrl")!;
                    var path = "/api/General/send-email";
                    client.CancelPendingRequests();
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    var payload = JsonConvert.SerializeObject(request);
                    var content = new StringContent(payload, Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenApp);
                    HttpResponseMessage response = await client.PostAsync(url + path, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = await response.Content.ReadAsStringAsync();
                        return "OK";
                    }
                    else
                    {
                        var readTask = await response.Content.ReadAsStringAsync();
                        return "ERROR";
                    }
                }
                catch (Exception error)
                {
                    return "ERROR";
                }
            }
        }

        public async Task<string> ConfirmPaymentOrder(string orderId, string tokenSession)
        {
            string url = "";
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {
                try
                {
                    dynamic request = new ExpandoObject();
                    request.orderId = orderId;

                    string tokenApp = await authService.GenerateTokenApplication();
                    url = configuration.GetValue<string>("globalVariables:hostUrl")!;
                    var path = "/api/Order/confirm";
                    client.CancelPendingRequests();
                    client.DefaultRequestHeaders.Clear();
                    client.Timeout = TimeSpan.FromSeconds(20);
                    var payload = JsonConvert.SerializeObject(request);
                    var content = new StringContent(payload, Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenApp);
                    client.DefaultRequestHeaders.Add("tokenSession", tokenSession);
                    HttpResponseMessage response = await client.PostAsync(url + path, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var readTask = await response.Content.ReadAsStringAsync();
                        return "OK";
                    }
                    else
                    {
                        var readTask = await response.Content.ReadAsStringAsync();
                        return "ERROR";
                    }
                }
                catch (Exception error)
                {
                    return "ERROR";
                }
            }
        }

    }

    public class DecimalFormatConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            FormattableString formattableString = $"{value:0.00}";
            writer.WriteRawValue(formattableString.ToString(CultureInfo.InvariantCulture));
        }

        public override bool CanRead => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Dialogflow.V2;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PhoneShop.Hubs;
using PhoneShop.Helper;

namespace PhoneShop.Controllers
{
    [ApiController]
    [Route("chatbot")]
    public class ChatbotController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ChatbotController> _logger;
        private readonly IHubContext<ChatHub> _hubContext; // Inject SignalR Hub

        public ChatbotController(HttpClient httpClient, ILogger<ChatbotController> logger, IHubContext<ChatHub> hubContext)
        {
            _httpClient = httpClient;
            _logger = logger;
            _hubContext = hubContext; // Gán SignalR Hub
        }

        [HttpPost]
        public async Task<IActionResult> HandleMessage([FromBody] ChatMessage message)
        {
            try
            {
                // Lấy Access Token
                var accessToken = await GetAccessTokenAsync();
                var customerIdClaim = HttpContext.User?.Claims.FirstOrDefault(c => c.Type == MySetting.CLAIM_CUSTOMERID)?.Value;
                if (string.IsNullOrEmpty(customerIdClaim))
                {
                    return BadRequest("Customer ID is missing.");
                }

                string sessionId = customerIdClaim; // Tạo session ID ngẫu nhiên


                var dialogflowRequest = new
                {
                    queryInput = new
                    {
                        text = new
                        {
                            text = message.Message,
                            languageCode = "en"
                        }
                    }
                };

                var dialogflowEndpoint = "https://dialogflow.googleapis.com/v2/projects/asp-mvc-with-website/agent/sessions/" + sessionId + ":detectIntent";
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, dialogflowEndpoint);
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(dialogflowRequest), Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(requestMessage);
                var responseBody = await response.Content.ReadAsStringAsync();

                string reply = null;
                if (response.IsSuccessStatusCode)
                {
                    var dialogflowResponse = JsonConvert.DeserializeObject<DialogflowResponse>(responseBody);
                    reply = dialogflowResponse.QueryResult.FulfillmentText;
                }
                else
                {
                    _logger.LogError("Error response from Dialogflow: {0}", responseBody);
                }

                if (!string.IsNullOrEmpty(customerIdClaim))
                {
                    await _hubContext.Clients.Group("Admin").SendAsync("ReceiveMessage", customerIdClaim, message.Message);
                }

                // Gửi phản hồi từ Dialogflow (nếu có) đến khách hàng
                if (!string.IsNullOrEmpty(reply))
                {
                    // Gửi tin nhắn phản hồi tới khung chat của admin (chỉ hiển thị trên UI admin)
                    await _hubContext.Clients.Group("Admin").SendAsync("ReceiveMessageToAdminChat", customerIdClaim, reply);



                }
                return Ok(new { reply });

            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while processing the request: {0}", ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Hàm lấy Access Token từ Service Account JSON
        private async Task<string> GetAccessTokenAsync()
        {
            var jsonPath = "C:\\Users\\trinh\\Downloads\\asp-mvc-with-website-a44fe81b47cb.json";  // Đường dẫn đến tệp JSON của Service Account

            // Tải credential từ file JSON
            var credential = GoogleCredential.FromFile(jsonPath)
                .CreateScoped("https://www.googleapis.com/auth/dialogflow");

            // Lấy Access Token
            var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            return token;
        }
    }


    public class ChatMessage
    {
        public string Message { get; set; }
    }

    public class DialogflowResponse
    {
        public QueryResult QueryResult { get; set; }
    }

    public class QueryResult
    {
        public string FulfillmentText { get; set; }
    }
}
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Dialogflow.V2;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PhoneShop.Controllers
{
    [ApiController]
    [Route("chatbot")]
    public class ChatbotController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ChatbotController> _logger;

        public ChatbotController(HttpClient httpClient, ILogger<ChatbotController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> HandleMessage([FromBody] ChatMessage message)
        {
            try
            {
                // Lấy Access Token
                var accessToken = await GetAccessTokenAsync();
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
                string sessionId = Guid.NewGuid().ToString(); // Tạo session ID ngẫu nhiên

                var dialogflowEndpoint = "https://dialogflow.googleapis.com/v2/projects/asp-mvc-with-website/agent/sessions/" + sessionId + ":detectIntent";
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, dialogflowEndpoint);
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(dialogflowRequest), Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(requestMessage);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error response from Dialogflow: {0}", responseBody);
                    return StatusCode((int)response.StatusCode, "Error from Dialogflow");
                }

                var dialogflowResponse = JsonConvert.DeserializeObject<DialogflowResponse>(responseBody);
                return Ok(new { reply = dialogflowResponse.QueryResult.FulfillmentText });
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
            var jsonPath = "C:\\Users\\Admin\\Downloads\\asp-mvc-with-website-a44fe81b47cb.json";  // Đường dẫn đến tệp JSON của Service Account

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

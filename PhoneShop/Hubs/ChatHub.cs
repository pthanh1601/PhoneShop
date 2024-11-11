using Microsoft.AspNetCore.SignalR;

namespace PhoneShop.Hubs
{
    public class ChatHub : Hub
    {
        // Phương thức nhận tin nhắn từ client gửi đến server
        public async Task SendMessageFromClient(string user, string message)
        {
            // Gửi tin nhắn của client cho server (hoặc tất cả client)
            await Clients.All.SendAsync("ReceiveClientMessage", user, message);
        }

        // Phương thức nhận tin nhắn từ server trả lời client
        public async Task SendMessageFromServer(string admin, string message)
        {
            // Gửi tin nhắn từ server cho tất cả client
            await Clients.All.SendAsync("ReceiveServerMessage", admin, message);
        }
    }
}

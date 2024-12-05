using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace PhoneShop.Hubs
{
    public class ChatHub : Hub
    {
        // Danh sách người dùng kết nối
        private static readonly Dictionary<string, string> ConnectedUsers = new();

        public override Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            ConnectedUsers[connectionId] = "Customer"; // Mặc định là khách hàng
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            ConnectedUsers.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        // Khách hàng gửi tin nhắn
        public async Task SendMessageToAdmin(string message)
        {
            await Clients.Group("Admin").SendAsync("ReceiveMessage", Context.ConnectionId, message);
        }

        // Admin gửi tin nhắn cho khách hàng cụ thể
        public async Task SendMessageToCustomer(string connectionId, string message)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", "Admin", message);
        }

        // Thêm người dùng vào nhóm Admin
        public Task AddToAdminGroup()
        {
            ConnectedUsers[Context.ConnectionId] = "Admin";
            return Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
        }
        // Gửi danh sách khách hàng đến Admin
        private Task UpdateAdminWithUserList()
        {
            var customerList = ConnectedUsers.Where(kvp => kvp.Value == "Customer")
                                             .Select(kvp => kvp.Key)
                                             .ToList();
            return Clients.Group("Admin").SendAsync("UpdateCustomerList", customerList);
        }
    }
}

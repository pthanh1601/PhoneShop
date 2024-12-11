using Microsoft.AspNetCore.SignalR;
using PhoneShop.Helper;
using System.Threading.Tasks;

namespace PhoneShop.Hubs
{
    public class ChatHub : Hub
    {
        // Danh sách người dùng kết nối
        private static readonly Dictionary<string, string> ConnectedUsers = new();

        public override async Task OnConnectedAsync()
        {
            string customerId = GetCustomerIdFromClaims();
            ConnectedUsers[Context.ConnectionId] = customerId; // Mặc định là khách hàng
            await base.OnConnectedAsync();
        }
        private string GetCustomerIdFromClaims()
        {
            // Lấy thông tin Claims từ context của người dùng đang kết nối
            var customerIdClaim = Context.User?.Claims.FirstOrDefault(c => c.Type == MySetting.CLAIM_CUSTOMERID);

            if (customerIdClaim != null)
            {
                return customerIdClaim.Value; // Trả về giá trị ID khách hàng từ claim
            }

            return null; // Nếu không tìm thấy, trả về null
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var customerId = GetCustomerIdFromClaims();
            if (customerId != null)
            {
                // Loại bỏ thông tin khách hàng theo CustomerId
                ConnectedUsers.Remove(customerId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        // Khách hàng gửi tin nhắn
        public async Task SendMessageToAdmin(string message)
        {
            var customerId = GetCustomerIdFromClaims();
            if (customerId != null)
            {
                // Gửi tin nhắn cho nhóm Admin
                await Clients.Group("Admin").SendAsync("ReceiveMessage", customerId, message);
            }
        }

        // Admin gửi tin nhắn cho khách hàng cụ thể
        public async Task SendMessageToCustomer(string customerId, string message)
        {
            // Lấy ConnectionId từ CustomerId
            var connectionId = ConnectedUsers.FirstOrDefault(x => x.Value == customerId).Key;

            if (connectionId != null)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", "Admin", message);
            }
        }

        // Thêm người dùng vào nhóm Admin
        public Task AddToAdminGroup()
        {
            var customerId = GetCustomerIdFromClaims();
            if (customerId != null)
            {
                ConnectedUsers[customerId] = "Admin";
            }

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
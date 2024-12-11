using Microsoft.AspNetCore.Mvc;
using PhoneShop.ViewModels;

namespace PhoneShop.ViewComponents
{
    public class RegistionViewComponent :ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var RegisterVM = new RegisterVM(); // Khởi tạo LoginVM nếu cần thiết
            return View(RegisterVM);
        }
    }
}

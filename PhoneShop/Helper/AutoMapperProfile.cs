using AutoMapper;
using PhoneShop.Data;
using PhoneShop.ViewModels;
namespace PhoneShop.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() {
            CreateMap<RegisterVM, KhachHang>();
        }
    }
}

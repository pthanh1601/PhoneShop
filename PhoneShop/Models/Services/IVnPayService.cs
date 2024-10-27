using PhoneShop.ViewModels;

namespace PhoneShop.Models.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);

        VnPaymentResponseModel PaymentExecute(IQueryCollection collection);

    }
}

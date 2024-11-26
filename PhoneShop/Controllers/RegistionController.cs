using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhoneShop.Data;
using PhoneShop.Helper;
using PhoneShop.ViewModels;

namespace PhoneShop.Controllers
{
    public class RegistionController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper;

        public RegistionController(Hshop2023Context context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }

        #region Registion
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(RegisterVM model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var khachHang = _mapper.Map<KhachHang>(model);
                    khachHang.RandomKey = MyUtil.GenerateRandomKey();
                    khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                    khachHang.HieuLuc = true;// xử lý khi dùng mail để active
                    khachHang.VaiTro = 0;

                    if (Hinh != null)
                    {
                        khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                    }

                    db.Add(khachHang);
                    db.SaveChanges();
                    return RedirectToAction("Index", "");
                }
                catch (Exception ex)
                {

                }
            }
            return View();
        }

        #endregion

    }
}

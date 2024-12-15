using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhoneShop.Data
{
    public partial class PhanCong
    {
        public required int MaPc { get; set; }
        
        public string MaNv { get; set; } = null!;


        public string MaPb { get; set; } = null!;

        public DateTime? NgayPc { get; set; }

        public bool? HieuLuc { get; set; }

        // Cho phép các trường điều hướng chấp nhận null
        public virtual NhanVien? MaNvNavigation { get; set; }

        public virtual PhongBan? MaPbNavigation { get; set; }
    }
}

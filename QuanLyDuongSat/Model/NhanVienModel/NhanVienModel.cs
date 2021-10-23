using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyDuongSat.Model.NhanVienModel
{
    public class NhanVienDangNhapModel
    {
        public string TaiKhoan { get; set; }
        public string MatKhau { get; set; }
    }

    public class NhanVienDashBoardModel
    {
        public int SoLuongTau { get; set; }
        public double TongDoanhThu { get; set; }
        public int TongSoGa { get; set; }
        public int TongSoKhachHang { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyDuongSat.Model.KhachHangModel
{
    public class KhachHangModel
    {
        public int MaKhach { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public int GioiTinh { get; set; }
        public string CMND { get; set; }
        public string SoDT { get; set; }
        public string MatKhau { get; set; }
    }

    public class KhachHangResponseModel
    {
        public int MaKhach { get; set; }
        public string HoTen { get; set; }
        public string SoDT { get; set; }
    }

    public class KhachHangDatVeModel
    {
        public int MaLoaiVe { get; set; }
        public int MaGhe { get; set; }
        public string HoTen { get; set; }
        public string CMND { get; set; }
        public string SoDT { get; set; }
    }
}
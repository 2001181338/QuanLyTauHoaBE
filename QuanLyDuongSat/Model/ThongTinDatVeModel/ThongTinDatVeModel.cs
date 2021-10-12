using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyDuongSat.Model.ThongTinDatVeModel
{
    public class ThongTinDatVeModel
    {
        public int MaChuyenTau { get; set; }
        public int LoaiVe { get; set; }
        public int SoLuongVeNam { get; set; }
        public int SoLuongVeNgoi { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public int GioiTinh { get; set; }
        public string CMND { get; set; }
        public string SoDT { get; set; }
        public string MatKhau { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyDuongSat.Model.VeTauModel
{
    public class VeTauModel
    {
        public int SoVe { get; set; }
        public string HoTen { get; set; }
        public string CMND { get; set; }
        public string SoDT { get; set; }
        public string LoaiVe { get; set; }
        public double GiaVe { get; set; }
        public int TrangThaiVe { get; set; }
        public string TenTau { get; set; }
        public string TenToa { get; set; }
        public string TenGhe { get; set; }
        public string GioKhoiHanh { get; set; }
        public string GaDi { get; set; }
        public string GaDen { get; set; }
        public DateTime? NgayKhoiHanh { get; set; }
        //public bool CoTheDoi { get; set; }
    }
}
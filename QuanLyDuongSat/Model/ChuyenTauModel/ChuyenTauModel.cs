using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyDuongSat.Model.ChuyenTauModel
{
    public class ChuyenTauModel
    {
        public int MaChuyenTau { get; set; }
        public string GaDi { get; set; }
        public string GaDen { get; set; }
        public string TenTau { get; set; }
        public DateTime? NgayKhoiHanh { get; set; }
        public string GioKhoiHanh { get; set; }
        public double GiaVeNgoi { get; set; }
        public double GiaVeNam { get; set; }
    }

    public class ChuyenTauTraVeModel
    {
        public int MaChuyenTau { get; set; }
        public DateTime NgayKhoiHanh { get; set; }
        public string GioKhoiHanh { get; set; }
    }
}
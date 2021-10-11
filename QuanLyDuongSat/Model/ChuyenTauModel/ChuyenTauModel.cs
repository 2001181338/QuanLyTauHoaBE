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
        public string NgayKhoiHanh { get; set; }
        public string GioKhoiHanh { get; set; }
        public double VeTapThe { get; set; }
        public double VeCaNhan { get; set; }
    }
}
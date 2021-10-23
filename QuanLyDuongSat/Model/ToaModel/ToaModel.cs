using QuanLyDuongSat.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyDuongSat.Model.ToaModel
{
    public class ToaGetAllModel
    {
        public int MaToa { get; set; }
        public string TenToa { get; set; }
        public LoaiToaTauEnum LoaiCho { get; set; }
        public int SoLuongGhe { get; set; }
        public int MaTau { get; set; }
        public string TenTau { get; set; }
    }

    public class ThemToaModel
    {
        public int MaTau { get; set; }
        public string TenToa { get; set; }
        public LoaiToaTauEnum LoaiToa { get; set; }
        public int SoLuongGhe { get; set; }
    }

    public class SuaToaModel
    {
        public int MaToa { get; set; }
        public int MaTau { get; set; }
        public string TenToa { get; set; }
        public LoaiToaTauEnum LoaiToa { get; set; }
        public int SoLuongGhe { get; set; }
    }

    public class ToaTimChuyenTauModel
    {
        public int MaToa { get; set; }
        public string TenToa { get; set; }
        public LoaiToaTauEnum LoaiToa { get; set; }
    }

    public class GetToaByChuyen
    {
        public int MaToa { get; set; }
        public int MaChuyenTau { get; set; }
    }
}

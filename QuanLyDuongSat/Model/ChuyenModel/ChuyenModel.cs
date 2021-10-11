using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyDuongSat.Model.ChuyenModel
{
    public class ChuyenGetAllModel
    {
        public int MaChuyen { get; set; }
        public int MaTuyen { get; set; }
        public int MaGaDi { get; set; }
        public int MaGaDen { get; set; }
        public string GioKhoiHanh { get; set; }
        public string TenGaDi { get; set; }
        public string TenGaDen { get; set; }
        public string TinhGaDi { get; set; }
        public string TinhGaDen { get; set; }
        public double TimeOrderBy { get; set; }
    }

    public class ChuyenThemModel
    {
        public int MaTuyen { get; set; }
        public string GioKhoiHanh { get; set; }
    }

    public class SuaChuyenModel
    {
        public int MaChuyen { get; set; }
        public int MaTuyen { get; set; }
        public string GioKhoiHanh { get; set; }
    }
}
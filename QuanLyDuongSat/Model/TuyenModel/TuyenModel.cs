using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyDuongSat.Model.TuyenModel
{
    public class TuyenGetAllModel
    {
        public int MaTuyen { get; set; }
        public int MaGaDi { get; set; }
        public int MaGaDen { get; set; }
        public string TenGaDi { get; set; }
        public string TenGaDen { get; set; }
        public int? MaTuyenCha { get; set; }
        public string TenTinhGaDi { get; set; }
        public string TenTinhGaDen { get; set; }
    }

    public class TuyenThemModel
    {
        public int MaGaDi { get; set; }
        public int MaGaDen { get; set; }
        public int? MaTuyenCha { get; set; }
    }

    public class TuyenSuaModel
    {
        public int MaTuyen { get; set; }
        public int MaGaDi { get; set; }
        public int MaGaDen { get; set; }
        public int? MaTuyenCha { get; set; }
    }

}
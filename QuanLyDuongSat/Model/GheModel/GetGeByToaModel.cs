using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyDuongSat.Model.GheModel
{
    public class GetGeByToaModel
    {
        public int MaGhe { get; set; }
        public string TenGhe { get; set; }
        public bool DaDat { get; set; }
        public double GiaVe { get; set; }
        public string TenToa { get; set; }
        public string SoCho { get; set; }
        public int MaLoaiVe { get; set; }
    }

    public class DanhSachDayGheModel
    {
        public List<GetGeByToaModel> GheDay0 { get; set; }
        public List<GetGeByToaModel> GheDay1 { get; set; }
        public List<GetGeByToaModel> GheDay2 { get; set; }
        public List<GetGeByToaModel> GheDay3 { get; set; }
    }
}
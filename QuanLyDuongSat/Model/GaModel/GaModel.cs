using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyDuongSat.Model.GaModel
{
    public class GaModel
    {
        public int MaGa { get; set; }
        public string TenGa { get; set; }
        public int MaThanhPhoTinh { get; set; }
        public string TenThanhPhoTinh { get; set; }
    }

    public class GaThemRequestModel
    {
        public int MaGa { get; set; }
        public string TenGa { get; set; }
        public int MaThanhPhoTinh { get; set; }
    }
}
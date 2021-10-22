using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyDuongSat.Model.VeModel
{
    public class VeModel
    {
        public int MaGa { get; set; }
        public string TenGa { get; set; }
        public int MaThanhPhoTinh { get; set; }
    }

    public class TraVeModel
    {
        public int MaVe { get; set; }
        public string CMND { get; set; }
        public string MaBaoMat { get; set; }
    }


    public class TraVeAdminModel
    {
        public int MaVe { get; set; }
        public string CMND { get; set; }
    }


    public class ThanhToanModel
    {
        public int MaVe { get; set; }
        public double SoDu { get; set; }
        public string CMND { get; set; }
        public string MaBaoMat { get; set; }
    }
}
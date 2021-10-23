using QuanLyDuongSat.Enumeration;
using QuanLyDuongSat.Model.KhachHangModel;
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

    public class DatVeModel
    {
        public int MaChuyenTau { get; set; }
        public LoaiVeEnum LoaiVe { get; set; }
        public bool IsTrucTiep { get; set; }
        public string CMND { get; set; }
        public string MaBaoMat { get; set; }
        public List<KhachHangDatVeModel> KhachHangGheModel { get; set; }
    }

    public class DatVeResponseModel
    {
        public int MaVe { get; set; }
        public DateTime NgayDat { get; set; }
        public TrangThaiVeEnum TrangThai { get; set; }
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
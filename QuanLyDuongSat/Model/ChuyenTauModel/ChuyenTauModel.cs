using QuanLyDuongSat.Enumeration;
using QuanLyDuongSat.Model.ToaModel;
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
        public double GiaVeNgoi { get; set; }
        public double GiaVeNam { get; set; }
        public bool HetHan { get; set; }
        public TrangThaiChuyenTauEnum TrangThai { get; set; }
        public string NgayGioKhoiHanh { get; set; }
        public List<ToaTimChuyenTauModel> Toas { get; set; }
        public string GiaVeNgoiStr { get => String.Format("{0:#,#.}", GiaVeNgoi); }
        public string GiaVeNamStr { get => String.Format("{0:#,#.}", GiaVeNam); }
    }

    public class ChuyenTauGetAllModel
    {
        public int MaChuyenTau { get; set; }
        public int MaGaDi { get; set; }
        public int MaGaDen { get; set; }
        public string GaDi { get; set; }
        public string GaDen { get; set; }
        public string GioKhoiHanh { get; set; }
        public DateTime? NgayKhoiHanh { get; set; }
        public string TenTau { get; set; }
        public int TongSoLuongGhe { get; set; }
        public int SoGheDaDat { get; set; }
        public double GiaVeNgoi { get; set; }
        public double GiaVeNam { get; set; }
        public int MaTau { get; set; }
        public int SoLuongToa { get; set; }
        public TrangThaiChuyenTauEnum TrangThaiTau { get; set; }
    }

    public class ChuyenTauThemModel
    {
        public int MaTau { get; set; }
        public int MaChuyen { get; set; }
        public DateTime NgayKhoiHanh { get; set; }
        public double VeNgoi { get; set; }
        public double VeNam { get; set; }
    }

    public class ChuyenTauTrangThaiModel
    {
        public int MaChuyenTau { get; set; }
        public TrangThaiChuyenTauEnum TrangThai { get; set; }
    }
}
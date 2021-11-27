using QuanLyDuongSat.Model.ResponseModel;
using QuanLyDuongSat.Model.TimKiemThongTinVeDatModel;
using QuanLyDuongSat.Model.VeTauModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QuanLyDuongSat.Controller
{
    public class TimKiemThongTinVeDatController : ApiController
    {
        [Route("api/timkiemthongtinvedat/timkiemve")]
        [HttpPost]
        public ResponseModel TimKiemVe(TimKiemThongTinVeDatModel model)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var thongTinKhachHang = db.KhachHangs.Where(x => x.CMND == model.CMND && x.SoDT == model.SoDT).ToList();
                if (!thongTinKhachHang.Any())
                {
                    return new ResponseModel
                    {
                        Data = null,
                        Message = "Chứng minh nhân dân hoặc số điện thoại không tồn tại trên hệ thống!",
                        Status = false
                    };
                }

                var allVe = db.Ves.ToList();
                var veTau = allVe.FirstOrDefault(x => thongTinKhachHang.Any(y => y.MaKhach == x.MaKhach) && (x.SoVe == model.SoVe));
                if (veTau == null)
                {
                    return new ResponseModel
                    {
                        Data = null,
                        Message = "Không tìm thấy vé tàu này",
                        Status = false
                    };
                }

                var khachHang = db.KhachHangs.FirstOrDefault(x => x.MaKhach == veTau.MaKhach);
                var loaiVe = db.LoaiVes.FirstOrDefault(x => x.MaLoaiVe == veTau.MaLoaiVe);

                var ghe = db.Ghes.FirstOrDefault(x => x.MaGhe == veTau.MaGhe);
                var toa = db.Toas.FirstOrDefault(x => x.MaToa == ghe.MaToa);
                var tau = db.Taus.FirstOrDefault(x => x.MaTau == toa.MaTau);
                var chuyenTau = db.ChuyenTaus.FirstOrDefault(x => x.MaChuyenTau == loaiVe.MaChuyenTau);
                var chuyen = db.Chuyens.FirstOrDefault(x => x.MaChuyen == chuyenTau.MaChuyen);
                var tuyen = db.Tuyens.FirstOrDefault(x => x.MaTuyen == chuyen.MaTuyen);
                var gaDi = db.Gas.FirstOrDefault(x => x.MaGa == tuyen.GaDi);
                var gaDen = db.Gas.FirstOrDefault(x => x.MaGa == tuyen.GaDen);


                //bool CoTheDoi  true;
                //CoTheDoi
                //if (gioDaTru < 86400)
                //{
                //    CoTheDoi  false;
                //}

                //1,2,3
                var VeTauModel = new VeTauModel()
                {
                    SoVe = veTau.SoVe,
                    CMND = khachHang.CMND,
                    SoDT = khachHang.SoDT,
                    HoTen = khachHang.HoTen,
                    LoaiVe = loaiVe.LoaiVe1 == 1 ? "Giường nằm" : "Ghế ngồi",
                    GiaVe = loaiVe.GiaVe ?? 0,
                    TrangThaiVe = veTau.TrangThai ?? 0,
                    TenGhe = ghe.TenGhe,
                    TenToa = toa.TenToa,
                    TenTau = tau.TenTau,
                    NgayKhoiHanh = chuyenTau.NgayKhoiHanh,
                    GioKhoiHanh = chuyen.GioKhoiHanh,
                    GaDi = gaDi.TenGa,
                    GaDen = gaDen.TenGa,
                    //CoTheDoi
                };


                return new ResponseModel
                {
                    Data = VeTauModel,
                    Message = "Có VÉ tàu cần tìm",
                    Status = true
                };
            }
        }


        [Route("api/timkiemthongtinvedat/get-danh-sach-ve")]
        [HttpPost]
        public ResponseModel GetDanhSachVeByMaVes(TimKiemThongTinVeDatBySoVesModel model)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {

                var allVe = db.Ves.ToList();
                var veTaus = allVe.Where(x => model.SoVes.Any(y => y == x.SoVe)).ToList();

                var listVeTau = new List<VeTauModel>();
                if (veTaus.Any())
                {
                    var allKhach = db.KhachHangs.ToList();
                    var allGhe = db.Ghes.ToList();
                    var allToa = db.Toas.ToList();
                    var allTau = db.Taus.ToList();
                    var allChuyenTau = db.ChuyenTaus.ToList();
                    var allChuyen = db.Chuyens.ToList();
                    var allGa = db.Gas.ToList();
                    var allLoaiVe = db.LoaiVes.ToList();
                    var allTuyen = db.Tuyens.ToList();

                    foreach (var veTau in veTaus)
                    {
                        var khachHang = allKhach.FirstOrDefault(x => x.MaKhach == veTau.MaKhach);
                        var loaiVe = allLoaiVe.FirstOrDefault(x => x.MaLoaiVe == veTau.MaLoaiVe);
                        var ghe = allGhe.FirstOrDefault(x => x.MaGhe == veTau.MaGhe);
                        var toa = allToa.FirstOrDefault(x => x.MaToa == ghe.MaToa);
                        var tau = allTau.FirstOrDefault(x => x.MaTau == toa.MaTau);
                        var chuyenTau = allChuyenTau.FirstOrDefault(x => x.MaChuyenTau == loaiVe.MaChuyenTau);
                        var chuyen = allChuyen.FirstOrDefault(x => x.MaChuyen == chuyenTau.MaChuyen);
                        var tuyen = allTuyen.FirstOrDefault(x => x.MaTuyen == chuyen.MaTuyen);
                        var gaDi = allGa.FirstOrDefault(x => x.MaGa == tuyen.GaDi);
                        var gaDen = allGa.FirstOrDefault(x => x.MaGa == tuyen.GaDen);

                        var veTauModel = new VeTauModel()
                        {
                            SoVe = veTau.SoVe,
                            CMND = khachHang != null ? khachHang.CMND : string.Empty,
                            SoDT = khachHang != null ? khachHang.SoDT : string.Empty,
                            HoTen = khachHang != null ? khachHang.HoTen : string.Empty,
                            LoaiVe = loaiVe != null ? (loaiVe.LoaiVe1 == 1 ? "Giường nằm" : "Ghế ngồi") : string.Empty,
                            GiaVe = loaiVe != null ? loaiVe.GiaVe ?? 0 : 0,
                            TrangThaiVe = veTau.TrangThaiVe ?? 0,
                            TenGhe = ghe != null ? ghe.TenGhe : string.Empty,
                            TenToa = toa != null ? toa.TenToa : string.Empty,
                            TenTau = tau != null ? tau.TenTau : string.Empty,
                            NgayKhoiHanh = chuyenTau != null ? chuyenTau.NgayKhoiHanh : null,
                            GioKhoiHanh = chuyen.GioKhoiHanh,
                            GaDi = gaDi.TenGa,
                            GaDen = gaDen.TenGa,
                            NgayBanVe = veTau.NgayBanVe,
                            TrangThai = veTau.TrangThai ?? 0,
                            SoCho = ghe.TenGhe.Substring(1)
                        };

                        listVeTau.Add(veTauModel);
                    }
                }

                return new ResponseModel
                {
                    Data = listVeTau.OrderByDescending(x => x.NgayBanVe).ToList(),
                    Message = "Có VÉ tàu cần tìm",
                    Status = true
                };
            }
        }
    }
}

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
                var thongTinKhachHang = db.KhachHangs.FirstOrDefault(x => x.CMND == model.CMND && x.SoDT == model.SoDT);
                if (thongTinKhachHang == null)
                {
                    return new ResponseModel
                    {
                        Data = null,
                        Message = "Chứng minh nhân dân hoặc số điện thoại không tồn tại trên hệ thống!",
                        Status = false
                    };
                }

                var khachHang = db.KhachHangs.Where(x => x.MaKhach == thongTinKhachHang.MaKhach).ToList();
                var allVe = db.Ves.ToList();
                var veTau = allVe.FirstOrDefault(x => khachHang.Any(y => y.MaKhach == x.MaKhach) && (x.SoVe == model.SoVe));
                if (veTau == null)
                {
                    return new ResponseModel
                    {
                        Data = null,
                        Message = "Mã số vé không hợp lệ!",
                        Status = false
                    };
                }

                var allKhach = db.KhachHangs.ToList();
                var cmnd = db.KhachHangs.FirstOrDefault(x => x.CMND == model.CMND);
                var sdt = db.KhachHangs.FirstOrDefault(x => x.SoDT == model.SoDT);

                var listVeTau = new List<VeTauModel>();

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
                    CMND = cmnd.CMND,
                    SoDT = sdt.SoDT,
                    HoTen = thongTinKhachHang.HoTen,
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
    }
}

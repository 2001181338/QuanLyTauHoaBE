using QuanLyDuongSat.Model.ChuyenTauModel;
using QuanLyDuongSat.Model.ResponseModel;
using QuanLyDuongSat.Model.TuyenModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QuanLyDuongSat.Controller
{
    public class TuyenController : ApiController
    {
        [Route("api/tuyen/timtuyen")]
        [HttpPost]
        public ResponseModel TimTuyen(TuyenModel model)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var tuyen = db.Tuyens.FirstOrDefault(x => x.GaDi == model.MaGaDi && x.GaDen == model.MaGaDen);
                if (tuyen == null)
                {
                    return new ResponseModel
                    {
                        Data = null,
                        Message = "Không có tuyến đường này!",
                        Status = false
                    };
                }

                var chuyens = db.Chuyens.Where(x => x.MaTuyen == tuyen.MaTuyen).ToList();
                if (!chuyens.Any())
                {
                    return new ResponseModel
                    {
                        Data = null,
                        Message = "Không có chuyến nào chạy trên tuyến đường này!",
                        Status = false
                    };
                }

                var allChuyenTau = db.ChuyenTaus.ToList();
                var chuyenTaus = allChuyenTau.Where(x => chuyens.Any(y => y.MaChuyen == x.MaChuyen) &&
                (x.NgayKhoiHanh.Value.Day == model.NgayKhoiHanh.Day && x.NgayKhoiHanh.Value.Month == model.NgayKhoiHanh.Month &&
                x.NgayKhoiHanh.Value.Year == model.NgayKhoiHanh.Year)).ToList();
                if (!chuyenTaus.Any())
                {
                    return new ResponseModel
                    {
                        Data = null,
                        Message = "Không có chuyến tàu nào chạy trong ngày này!",
                        Status = false
                    };
                }

                var allTau = db.Taus.ToList();
                var gaDi = db.Gas.FirstOrDefault(x => x.MaGa == model.MaGaDi);
                var gaDen = db.Gas.FirstOrDefault(x => x.MaGa == model.MaGaDen);

                var listChuyenTau = new List<ChuyenTauModel>();

                var loaiVes = db.LoaiVes.ToList();

                foreach (var chuyenTau in chuyenTaus)
                {
                    var chuyenTauModel = new ChuyenTauModel()
                    {
                        MaChuyenTau = chuyenTau.MaChuyenTau,
                        GaDi = gaDi.TenGa,
                        GaDen = gaDen.TenGa,
                        GioKhoiHanh = chuyens.FirstOrDefault(x => x.MaChuyen == chuyenTau.MaChuyen)?.GioKhoiHanh,
                        NgayKhoiHanh = chuyenTau.NgayKhoiHanh.ToString(),
                        TenTau = allTau.FirstOrDefault(x => x.MaTau == chuyenTau.MaTau)?.TenTau,
                        VeTapThe = loaiVes.FirstOrDefault(x => x.MaChuyenTau == chuyenTau.MaChuyenTau && x.LoaiVe1 == 1).GiaVe ?? 0,
                        VeCaNhan = loaiVes.FirstOrDefault(x => x.MaChuyenTau == chuyenTau.MaChuyenTau && x.LoaiVe1 == 2).GiaVe ?? 0
                    };

                    listChuyenTau.Add(chuyenTauModel);
                }

                return new ResponseModel
                {
                    Data = listChuyenTau,
                    Message = "Có chuyến tàu cần tìm",
                    Status = true
                };
            }
        }
    }
}

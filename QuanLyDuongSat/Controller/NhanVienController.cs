using QuanLyDuongSat.Enumeration;
using QuanLyDuongSat.GlobalVariable;
using QuanLyDuongSat.Model.NhanVienModel;
using QuanLyDuongSat.Model.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace QuanLyDuongSat.Controller
{
    [RoutePrefix("api/nhanvien")]
    public class NhanVienController : ApiController
    {
        [Route("dangnhap")]
        [HttpPost]
        public ResponseModel DangNhap(NhanVienDangNhapModel model)
        {
            if (string.IsNullOrEmpty(model.TaiKhoan) || string.IsNullOrEmpty(model.MatKhau))
            {
                return new ResponseModel
                {
                    Data = null,
                    Message = "Vui lòng không để trống",
                    Status = false
                };
            }
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var nhanVienCheck = db.QuanTris.FirstOrDefault(x => x.TaiKhoan.ToLower().Trim() == model.TaiKhoan.ToLower().Trim() && x.MatKhau.ToLower().Trim() == model.MatKhau.ToLower().Trim());
                if (nhanVienCheck == null)
                {
                    return new ResponseModel
                    {
                        Data = null,
                        Message = "Tên đăng nhập hoặc mật khẩu không đúng",
                        Status = false
                    };
                }

                TaiKhoanDangNhap.TaiKhoan = nhanVienCheck.TaiKhoan;

                return new ResponseModel
                {
                    Data = model,
                    Message = "Đăng nhập thành công",
                    Status = true
                };
            }
        }

        [Route("dashboard")]
        [HttpGet]
        public ResponseModel GetInfoDashBoard()
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var veDaThanhToan = db.Ves.Where(x => x.TrangThai == (int)TrangThaiVeEnum.DaThanhToan).ToList();
                var VeDaTinhPhi = db.Ves.Where(x => x.PhiTraVe != null && x.PhiTraVe != 0).ToList();
                var loaiVes = db.LoaiVes.ToList();

                double doanhThu = 0;
                foreach (var ve in veDaThanhToan)
                {
                    var loaiVe = loaiVes.FirstOrDefault(x => x.MaLoaiVe == ve.MaLoaiVe);
                    if (loaiVe != null)
                    {
                        doanhThu += loaiVe.GiaVe ?? 0;
                    }
                }

                doanhThu += VeDaTinhPhi.Sum(x => x.PhiTraVe) ?? 0;

                var res = new NhanVienDashBoardModel()
                {
                    SoLuongTau = db.Taus.Count(),
                    TongSoGa = db.Gas.Count(),
                    TongSoKhachHang = db.KhachHangs.Count(),
                    TongDoanhThu = doanhThu
                };

                return new ResponseModel()
                {
                    Status = true,
                    Data = res
                };
            }
        }

        [Route("get-all")]
        [HttpGet]
        public ResponseModel GetAllNhanVien()
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var nhanViens = db.QuanTris.Select(x => new NhanVienGetAllModel()
                {
                    TaiKhoan = x.TaiKhoan,
                    NgayLap = x.NgayLap
                }).ToList();

                return new ResponseModel()
                {
                    Status = true,
                    Data = nhanViens
                };
            }
        }

        [Route("thong-ke/{nam}")]
        [HttpGet]
        public ResponseModel GetAllThongKeTheoNam(int nam)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var ves = db.Ves.Where(x => x.NgayBanVe.Value.Year == nam && (x.TrangThai == (int)TrangThaiVeEnum.DaThanhToan || (x.TrangThai == (int)TrangThaiVeEnum.DaHuy && x.PhiTraVe != 0))).ToList();
                var listThangDoanhThu = new List<NhanVienThongKeThangModel>();
                var allLoaiVe = db.LoaiVes.ToList();
                var lstLoaiVe = allLoaiVe.Where(x => ves.Any(y => y.MaLoaiVe == x.MaLoaiVe)).ToList();
                double giaMax = 0;

                for (int i = 1; i <= 12; i++)
                {
                    var veThang = ves.Where(x => x.NgayBanVe.Value.Month == i).ToList();
                    double doanhThu = 0;
                    if (veThang.Any())
                    {
                        foreach (var ve in veThang)
                        {
                            var loaiVe = lstLoaiVe.FirstOrDefault(x => x.MaLoaiVe == ve.MaLoaiVe);
                            if (loaiVe != null)
                            {
                                doanhThu += loaiVe.GiaVe ?? 0;
                            }
                        }
                    }

                    if (doanhThu > giaMax) giaMax = doanhThu;

                    var thang = new NhanVienThongKeThangModel()
                    {
                        Thang = i,
                        DoanhThu = doanhThu
                    };

                    listThangDoanhThu.Add(thang);
                }

                foreach(var thang in listThangDoanhThu)
                {
                    if(giaMax != 0 && thang.DoanhThu == giaMax)
                    {
                        thang.IsMax = true;
                        break;
                    }
                }

                return new ResponseModel()
                {
                    Status = true,
                    Data = listThangDoanhThu
                };
            }
        }
    }
}

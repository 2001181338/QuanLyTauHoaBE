using QuanLyDuongSat.Enumeration;
using QuanLyDuongSat.Model.ResponseModel;
using QuanLyDuongSat.Model.VeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QuanLyDuongSat.Controller
{
    public class VeController : ApiController
    {
        [Route("api/ve/get-danh-sach-ga")]
        [HttpGet]
        public ResponseModel GetDanhSachGa()
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var danhsachGa = db.Gas.ToList().Select(x => new VeModel()
                {
                    MaGa = x.MaGa,
                    TenGa = x.TenGa,
                    MaThanhPhoTinh = (int)x.MaThanhPhoTinh
                }).ToList();
                return new ResponseModel()
                {
                    Data = danhsachGa.ToList(),
                    Status = true
                };
            }
        }

        [Route("api/ve/dat-ve")]
        [HttpPost]
        public ResponseModel DatVe(DatVeModel model)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var chuyenTau = db.ChuyenTaus.FirstOrDefault(x => x.MaChuyenTau == model.MaChuyenTau);
                if (chuyenTau == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Chuyến tàu không tồn tại"
                    };
                }

                var loaiVes = db.LoaiVes.Where(x => x.MaChuyenTau == chuyenTau.MaChuyenTau).ToList();
                var listVeDB = db.Ves.ToList();
                var listVe = listVeDB.Where(x => loaiVes.Any(y => y.MaLoaiVe == x.MaLoaiVe) && (x.TrangThai == (int)TrangThaiVeEnum.DaThanhToan || x.TrangThai == (int)TrangThaiVeEnum.ChuaThanhToan)).ToList();
                //Kiểm tra các ghế đã được đặt chưa
                if(listVe.Any(x => model.KhachHangGheModel.Any(y => y.MaGhe == x.MaGhe)))
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Đã có ghế được đặt, vui lòng làm mới lại trang"
                    };
                }

                var chuyen = db.Chuyens.FirstOrDefault(x => x.MaChuyen == chuyenTau.MaChuyen);
                if (chuyen == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Chuyến không tồn tại"
                    };
                }

                var ngayKhoiHanh = DateTime.Parse(chuyenTau.NgayKhoiHanh.ToString().Substring(0, 10) + " " + chuyen.GioKhoiHanh);
                var gioConvertSeconds = (ngayKhoiHanh - DateTime.Now).TotalSeconds;
                if (gioConvertSeconds < 86400)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Vui lòng đặt vé trước 24, trước khi tàu khởi hành"
                    };
                }

                //Check ngan hang
                if (!model.IsTrucTiep)
                {
                    var nganHang = db.NganHangs.FirstOrDefault(x => x.CMND == model.CMND && x.MaBaoMat == model.MaBaoMat);
                    if (nganHang == null)
                    {
                        return new ResponseModel()
                        {
                            Status = false,
                            Message = "CMND/CCCD hoặc mã bảo mật không đúng"
                        };
                    }

                    double tongTien = 0;
                    //Kiểm tra số dư, nếu số dư ko đủ => báo lỗi
                    foreach (var item in model.KhachHangGheModel)
                    {
                        var loaiVe = loaiVes.FirstOrDefault(x => x.MaLoaiVe == item.MaLoaiVe);
                        if(loaiVe != null)
                        {
                            tongTien += loaiVe.GiaVe ?? 0;
                        }
                    }

                    if(tongTien > nganHang.SoDu)
                    {
                        return new ResponseModel()
                        {
                            Status = false,
                            Message = "Số dư của tài khoản không đủ vui lòng chọn phương thức thanh toán khác"
                        };
                    }

                    nganHang.SoDu -= tongTien;
                }

                var listNewVe = new List<Ve>();

                foreach (var item in model.KhachHangGheModel)
                {
                    var newKhachHang = new KhachHang()
                    {
                        CMND = item.CMND,
                        HoTen = item.HoTen,
                        SoDT = item.SoDT
                    };

                    db.KhachHangs.InsertOnSubmit(newKhachHang);
                    db.SubmitChanges();

                    var newVe = new Ve()
                    {
                        MaLoaiVe = item.MaLoaiVe,
                        TrangThaiVe = (int)model.LoaiVe,
                        TrangThai = model.IsTrucTiep ? (int)TrangThaiVeEnum.ChuaThanhToan : (int)TrangThaiVeEnum.DaThanhToan,
                        MaKhach = newKhachHang.MaKhach,
                        NgayBanVe = DateTime.Now,
                        MaGhe = item.MaGhe
                    };

                    listNewVe.Add(newVe);
                }

                if (listNewVe.Any())
                {
                    db.Ves.InsertAllOnSubmit(listNewVe);
                    db.SubmitChanges();
                }

                var listResponseModel = new List<DatVeResponseModel>();
                foreach(var ve in listNewVe)
                {
                    var newRes = new DatVeResponseModel()
                    {
                        MaVe  = ve.SoVe,
                        NgayDat = DateTime.Now,
                        TrangThai = (TrangThaiVeEnum)ve.TrangThai
                    };

                    listResponseModel.Add(newRes);
                }

                return new ResponseModel()
                {
                    Status = true,
                    Message = "Đặt chỗ thành công",
                    Data = listResponseModel
                };
            }
        }
    }
}

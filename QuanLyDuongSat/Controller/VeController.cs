using QuanLyDuongSat.Enumeration;
using QuanLyDuongSat.Model.QuanLyVeTauModel;
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
                var tinhThanhPhos = db.ThanhPho_Tinhs.ToList();
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
                if (listVe.Any(x => model.KhachHangGheModel.Any(y => y.MaGhe == x.MaGhe)))
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
                        if (loaiVe != null)
                        {
                            tongTien += loaiVe.GiaVe ?? 0;
                        }
                    }

                    if (tongTien > nganHang.SoDu)
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
                foreach (var ve in listNewVe)
                {
                    var newRes = new DatVeResponseModel()
                    {
                        MaVe = ve.SoVe,
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

        [Route("api/ve/get-danh-sach-ve-tau")]
        [HttpGet]
        public ResponseModel GetDanhSachVeTau()
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var veTau = from ve in db.Ves
                            join ghe in db.Ghes on ve.MaGhe equals ghe.MaGhe
                            join toa in db.Toas on ghe.MaToa equals toa.MaToa
                            join tau in db.Taus on toa.MaTau equals tau.MaTau
                            join khachHang in db.KhachHangs on ve.MaKhach equals khachHang.MaKhach
                            join loaiVe in db.LoaiVes on ve.MaLoaiVe equals loaiVe.MaLoaiVe
                            join chuyenTau in db.ChuyenTaus on loaiVe.MaChuyenTau equals chuyenTau.MaChuyenTau
                            join chuyen in db.Chuyens on chuyenTau.MaChuyen equals chuyen.MaChuyen
                            join tuyen in db.Tuyens on chuyen.MaTuyen equals tuyen.MaTuyen
                            join gaDi in db.Gas on tuyen.GaDi equals gaDi.MaGa
                            join gaDen in db.Gas on tuyen.GaDen equals gaDen.MaGa
                            select new QuanLyVeTauModel
                            {
                                SoVe = ve.SoVe,
                                HoTen = khachHang.HoTen,
                                CMND = khachHang.CMND,
                                SoDT = khachHang.SoDT,
                                TenGhe = ghe.TenGhe,
                                TenToa = toa.TenToa,
                                TenTau = tau.TenTau,
                                GaDi = gaDi.TenGa,
                                GaDen = gaDen.TenGa,
                                NgayKhoiHanh = chuyenTau.NgayKhoiHanh,
                                GioKhoiHanh = chuyen.GioKhoiHanh,
                                NgayBanVe = ve.NgayBanVe,
                                GiaVe = loaiVe.GiaVe ?? 0,
                                TrangThaiVe = ve.TrangThaiVe ?? 0,
                                LoaiVe = loaiVe.LoaiVe1 ?? 0,
                                TrangThai = ve.TrangThai ?? 0,
                            };
                return new ResponseModel()
                {
                    Status = true,
                    Data = veTau.ToList()
                };
            }
        }


        [Route("api/ve/tra-ve")]
        [HttpPost]
        public ResponseModel TraVe(TraVeModel model)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var ve = db.Ves.FirstOrDefault(x => x.SoVe == model.MaVe);
                if (ve == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Mã vé không hợp lệ!"
                    };
                }

                //Kiem tra thong tin bao mat
                var nganHang = db.NganHangs.FirstOrDefault(x => x.CMND.Trim() == model.CMND.Trim() && x.MaBaoMat == model.MaBaoMat);
                if (nganHang == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Tài khoản hoặc mã bảo mật không đúng!"
                    };
                }

                //Kiem tra ngay dat ve va gio khoi hanh
                //Neu ..
                var loaiVe = db.LoaiVes.FirstOrDefault(x => x.MaLoaiVe == ve.MaLoaiVe);
                var chuyenTau = db.ChuyenTaus.FirstOrDefault(x => x.MaChuyenTau == loaiVe.MaChuyenTau);
                var chuyen = db.Chuyens.FirstOrDefault(x => x.MaChuyen == chuyenTau.MaChuyen);

                var sNgayKhoiHanh = chuyenTau.NgayKhoiHanh.ToString().Substring(0, 10) + " " + chuyen.GioKhoiHanh;
                var ngayKhoiHanhConvert = DateTime.Parse(sNgayKhoiHanh);
                var gioDaTru = (ngayKhoiHanhConvert - DateTime.Now).TotalSeconds;
                var seconds = 3600;

                //Vé cá nhân
                if (ve.TrangThaiVe == (int)LoaiVeEnum.CaNhan)
                {
                    //Trường hợp không thể trả vé: trả vé trước giờ tàu chạy dưới 4 tiếng
                    if (gioDaTru < 4 * seconds)
                    {
                        return new ResponseModel()
                        {
                            Status = false,
                            Message = "Vé tàu (cá nhân) của bạn đã hết thời hạn trả!"
                        };
                    }
                    //Trả vé trước giờ tàu chạy từ 4 giờ đến dưới 48 giờ, lệ phí là 20% giá vé
                    else if (gioDaTru < 48 * seconds)
                    {
                        ve.TrangThai = (int)TrangThaiVeEnum.DaHuy;
                        nganHang.SoDu += (loaiVe.GiaVe - loaiVe.GiaVe * 0.2) ?? 0;
                        ve.PhiTraVe = loaiVe.GiaVe * 0.2;
                    }
                    //Trả vé từ 48 giờ trở lên lệ phí là 10% giá vé
                    else
                    {
                        ve.TrangThai = (int)TrangThaiVeEnum.DaHuy;
                        nganHang.SoDu += (loaiVe.GiaVe - loaiVe.GiaVe * 0.1) ?? 0;
                        ve.PhiTraVe = loaiVe.GiaVe * 0.1;
                    }
                }
                //Vé tập thể
                else
                {
                    //Trường hợp không thể trả vé: trả vé trước giờ tàu chạy dưới 24 tiếng
                    if (gioDaTru < 24 * seconds)
                    {
                        return new ResponseModel()
                        {
                            Status = false,
                            Message = "Vé tàu (tập thể) của bạn đã hết thời hạn trả!"
                        };
                    }
                    //Trả vé trước giờ tàu chạy từ 24 giờ đến dưới 72 giờ, lệ phí là 30% giá vé
                    else if (gioDaTru < 72 * seconds)
                    {
                        ve.TrangThai = (int)TrangThaiVeEnum.DaHuy;
                        nganHang.SoDu += (loaiVe.GiaVe - loaiVe.GiaVe * 0.3) ?? 0;
                        ve.PhiTraVe = loaiVe.GiaVe * 0.3;
                    }
                    //Trả vé từ 72 giờ trở lên lệ phí là 20% giá vé
                    else
                    {
                        ve.TrangThai = (int)TrangThaiVeEnum.DaHuy;
                        nganHang.SoDu += (loaiVe.GiaVe - loaiVe.GiaVe * 0.2) ?? 0;
                        ve.PhiTraVe = loaiVe.GiaVe * 0.2;
                    }
                }

                db.SubmitChanges();
            }

            return new ResponseModel()
            {
                Status = true,
                Message = "Trả vé thành công"
            };
        }


        [Route("api/ve/tra-ve-admin/{maVe}")]
        [HttpPost]
        public ResponseModel TraVeAdmin(int maVe)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var ve = db.Ves.FirstOrDefault(x => x.SoVe == maVe);
                if (ve == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Mã vé không hợp lệ!"
                    };
                }

                //Kiem tra ngay dat ve va gio khoi hanh
                //Neu ..
                var loaiVe = db.LoaiVes.FirstOrDefault(x => x.MaLoaiVe == ve.MaLoaiVe);
                var chuyenTau = db.ChuyenTaus.FirstOrDefault(x => x.MaChuyenTau == loaiVe.MaChuyenTau);
                var chuyen = db.Chuyens.FirstOrDefault(x => x.MaChuyen == chuyenTau.MaChuyen);

                var sNgayKhoiHanh = chuyenTau.NgayKhoiHanh.ToString().Substring(0, 10) + " " + chuyen.GioKhoiHanh;
                var ngayKhoiHanhConvert = DateTime.Parse(sNgayKhoiHanh);
                var gioDaTru = (ngayKhoiHanhConvert - DateTime.Now).TotalSeconds;
                var seconds = 3600;

                //Vé cá nhân
                if (ve.TrangThaiVe == (int)LoaiVeEnum.CaNhan)
                {
                    //Trường hợp không thể trả vé: trả vé trước giờ tàu chạy dưới 4 tiếng
                    if (gioDaTru < 4 * seconds)
                    {
                        return new ResponseModel()
                        {
                            Status = false,
                            Message = "Vé tàu (cá nhân) của bạn đã hết thời hạn trả!"
                        };
                    }
                    //Trả vé trước giờ tàu chạy từ 4 giờ đến dưới 48 giờ, lệ phí là 20% giá vé
                    else if (gioDaTru < 48 * seconds)
                    {
                        ve.TrangThai = (int)TrangThaiVeEnum.DaHuy;
                        double phiTraVe = (loaiVe.GiaVe ?? 0) * 0.2;
                        ve.PhiTraVe = phiTraVe;
                    }
                    //Trả vé từ 48 giờ trở lên lệ phí là 10% giá vé
                    else
                    {
                        ve.TrangThai = (int)TrangThaiVeEnum.DaHuy;
                        double phiTraVe = (loaiVe.GiaVe ?? 0) * 0.1;
                        ve.PhiTraVe = phiTraVe;
                    }
                }
                //Vé tập thể
                else
                {
                    //Trường hợp không thể trả vé: trả vé trước giờ tàu chạy dưới 24 tiếng
                    if (gioDaTru < 24 * seconds)
                    {
                        return new ResponseModel()
                        {
                            Status = false,
                            Message = "Vé tàu (tập thể) của bạn đã hết thời hạn trả!"
                        };
                    }
                    //Trả vé trước giờ tàu chạy từ 24 giờ đến dưới 72 giờ, lệ phí là 30% giá vé
                    else if (gioDaTru < 72 * seconds)
                    {
                        ve.TrangThai = (int)TrangThaiVeEnum.DaHuy;
                        double phiTraVe = (loaiVe.GiaVe ?? 0) * 0.3;
                        ve.PhiTraVe = phiTraVe;
                    }
                    //Trả vé từ 72 giờ trở lên lệ phí là 20% giá vé
                    else
                    {
                        ve.TrangThai = (int)TrangThaiVeEnum.DaHuy;
                        double phiTraVe = (loaiVe.GiaVe ?? 0) * 0.2;
                        ve.PhiTraVe = phiTraVe;
                    }
                }

                db.SubmitChanges();
            }

            return new ResponseModel()
            {
                Status = true,
                Message = "Trả vé thành công"
            };
        }


        [Route("api/ve/thanh-toan")]
        [HttpPost]
        public ResponseModel ThanhToan(ThanhToanModel model)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var ve = db.Ves.FirstOrDefault(x => x.SoVe == model.MaVe);
                if (ve == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Mã vé không hợp lệ!"
                    };
                }

                //Kiem tra thong tin bao mat
                var nganHang = db.NganHangs.FirstOrDefault(x => x.CMND.Trim() == model.CMND.Trim() && x.MaBaoMat == model.MaBaoMat);
                if (nganHang == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "CMND hoặc Mã bảo mật không hợp lệ!"
                    };
                }

                var loaiVe = db.LoaiVes.FirstOrDefault(x => x.MaLoaiVe == ve.MaLoaiVe);
                var giaVe = loaiVe.GiaVe;
                var soDu = nganHang.SoDu;

                if (soDu < giaVe)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Số dư trong tài khoản của bạn không đủ để thanh toán!"
                    };
                }
                else
                {
                    ve.TrangThai = (int)TrangThaiVeEnum.DaThanhToan;
                    nganHang.SoDu -= giaVe ?? 0;
                    //ve.TienThanhToan += giaVe;
                }

                db.SubmitChanges();
            }
            return new ResponseModel()
            {
                Status = true,
                Message = "Thanh toán thành công"
            };
        }


        [Route("api/ve/thanh-toan-admin")]
        [HttpPost]
        public ResponseModel ThanhToanAdmin(TraVeAdminModel model)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var ve = db.Ves.FirstOrDefault(x => x.SoVe == model.MaVe);
                if (ve == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Mã vé không hợp lệ!"
                    };
                }

                //Kiem tra thong tin bao mat
                var nganHang = db.NganHangs.FirstOrDefault(x => x.CMND.Trim() == model.CMND.Trim());
                if (nganHang == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "CMND không hợp lệ!"
                    };
                }

                //var loaiVe = db.LoaiVes.FirstOrDefault(x => x.MaLoaiVe == ve.MaLoaiVe);
                //var giaVe = loaiVe.GiaVe;
                //var soDu = nganHang.SoDu;

                //if (soDu < giaVe)
                //{
                //    return new ResponseModel()
                //    {
                //        Status = false,
                //        Message = "Số dư trong tài khoản của bạn không đủ để thanh toán!"
                //    };
                //}
                //else
                //{
                //    ve.TrangThai = (int)TrangThaiVeEnum.DaThanhToan;
                //    nganHang.SoDu -= giaVe;
                //}

                ve.TrangThai = (int)TrangThaiVeEnum.DaThanhToan;

                db.SubmitChanges();
            }
            return new ResponseModel()
            {
                Status = true,
                Message = "Thanh toán thành công"
            };
        }

        [Route("api/ve/doi-ve-admin/{maVe}")]
        [HttpPost]
        public ResponseModel DoiVeAdmin(int maVe)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var ve = db.Ves.FirstOrDefault(x => x.SoVe == maVe);
                if (ve == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Mã vé không hợp lệ!"
                    };
                }

                //Vé cá nhân
                if (ve.TrangThaiVe == (int)LoaiVeEnum.TapThe)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Không áp dụng đối với vé tập thể!"
                    };
                }

                //Kiem tra ngay dat ve va gio khoi hanh
                //Neu ..
                var loaiVe = db.LoaiVes.FirstOrDefault(x => x.MaLoaiVe == ve.MaLoaiVe);
                var chuyenTau = db.ChuyenTaus.FirstOrDefault(x => x.MaChuyenTau == loaiVe.MaChuyenTau);
                var chuyen = db.Chuyens.FirstOrDefault(x => x.MaChuyen == chuyenTau.MaChuyen);

                var sNgayKhoiHanh = chuyenTau.NgayKhoiHanh.ToString().Substring(0, 10) + " " + chuyen.GioKhoiHanh;
                var ngayKhoiHanhConvert = DateTime.Parse(sNgayKhoiHanh);
                var gioDaTru = (ngayKhoiHanhConvert - DateTime.Now).TotalSeconds;
                var seconds = 3600;

                if (gioDaTru < 86400)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Đã quá giờ đổi vé!"
                    };
                }

                ve.TrangThai = (int)TrangThaiVeEnum.DaHuy;
                ve.PhiTraVe = 20000;
                db.SubmitChanges();

                return new ResponseModel()
                {
                    Status = true,
                    Message = "Đổi vé thành công!"
                };
            }
        }

        [Route("api/ve/thanh-toan-ve-admin/{maVe}")]
        [HttpPost]
        public ResponseModel ThanhToanVeAdmin(int maVe)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var ve = db.Ves.FirstOrDefault(x => x.SoVe == maVe);
                if (ve == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Mã vé không hợp lệ!"
                    };
                }
                ve.TrangThai = (int)TrangThaiVeEnum.DaThanhToan;

                db.SubmitChanges();
            }
            return new ResponseModel()
            {
                Status = true,
                Message = "Thanh toán thành công"
            };
        }


    }
}

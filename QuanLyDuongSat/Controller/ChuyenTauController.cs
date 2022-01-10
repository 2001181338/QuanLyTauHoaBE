using QuanLyDuongSat.Enumeration;
using QuanLyDuongSat.GlobalVariable;
using QuanLyDuongSat.Model.ChuyenTauModel;
using QuanLyDuongSat.Model.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QuanLyDuongSat.Controller
{
    [RoutePrefix("api/chuyentau")]
    public class ChuyenTauController : ApiController
    {
        [Route("get-all")]
        [HttpGet]
        public ResponseModel GetAll()
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var chuyenTaus = from chuyenTau in db.ChuyenTaus
                                 join chuyen in db.Chuyens on chuyenTau.MaChuyen equals chuyen.MaChuyen
                                 join tuyen in db.Tuyens on chuyen.MaTuyen equals tuyen.MaTuyen
                                 join gaDi in db.Gas on tuyen.GaDi equals gaDi.MaGa
                                 join gaDen in db.Gas on tuyen.GaDen equals gaDen.MaGa
                                 join tau in db.Taus on chuyenTau.MaTau equals tau.MaTau
                                 select new ChuyenTauGetAllModel()
                                 {
                                     MaChuyenTau = chuyenTau.MaChuyenTau,
                                     GaDi = gaDi.TenGa,
                                     GaDen = gaDen.TenGa,
                                     NgayKhoiHanh = chuyenTau.NgayKhoiHanh,
                                     GioKhoiHanh = chuyen.GioKhoiHanh,
                                     TenTau = tau.TenTau,
                                     MaTau = tau.MaTau,
                                     MaGaDi = gaDi.MaGa,
                                     MaGaDen = gaDen.MaGa,
                                     TrangThaiTau = (TrangThaiChuyenTauEnum)chuyenTau.TrangThai,
                                     MaTuyen = tuyen.MaTuyen
                                 };

                var lstChuyenTau = chuyenTaus.ToList();
                var loaiVes = db.LoaiVes.ToList();
                var ves = db.Ves.ToList();
                var toas = db.Toas.ToList();
                var ghes = db.Ghes.ToList();
                var tuyens = db.Tuyens.ToList();
                var chuyens = db.Chuyens.ToList();
                var chuyenTauAll = db.ChuyenTaus.ToList();

                foreach (var chuyenTau in lstChuyenTau)
                {
                    var toaTemps = toas.Where(x => x.MaTau == chuyenTau.MaTau).ToList();
                    var gheTemps = ghes.Where(x => toaTemps.Any(y => y.MaToa == x.MaToa)).ToList();

                    chuyenTau.TongSoLuongGhe = gheTemps.Count();
                    chuyenTau.SoLuongToa = toaTemps.Count();

                    //Lay gia ve
                    var loaiVeTemps = loaiVes.Where(x => x.MaChuyenTau == chuyenTau.MaChuyenTau).ToList();
                    if (loaiVeTemps.Any())
                    {
                        var veNgoi = loaiVeTemps.FirstOrDefault(x => x.LoaiVe1 == (int)LoaiToaTauEnum.ToaNgoi);
                        var veNam = loaiVeTemps.FirstOrDefault(x => x.LoaiVe1 == (int)LoaiToaTauEnum.ToaNam);

                        if (veNgoi != null)
                        {
                            chuyenTau.GiaVeNgoi = veNgoi.GiaVe ?? 0;
                        }

                        if (veNam != null)
                        {
                            chuyenTau.GiaVeNam = veNam.GiaVe ?? 0;
                        }
                    }

                    //Lay so luong ve da duoc dat
                    var veTemps = ves.Where(x => loaiVeTemps.Any(y => y.MaLoaiVe == x.MaLoaiVe) && (x.TrangThai == (int)TrangThaiVeEnum.DaThanhToan || x.TrangThai == (int)TrangThaiVeEnum.ChuaThanhToan)).ToList();
                    if (veTemps.Any())
                    {
                        chuyenTau.SoGheDaDat = veTemps.Count();
                    }
                    var tuyen = tuyens.FirstOrDefault(x => x.MaTuyen == chuyenTau.MaTuyen);

                    if (tuyen != null)
                    {
                        //Kiểm tra tuyến cha
                        if (tuyen.TuyenCha != null)
                        {
                            var chuyenCha = chuyens.Where(x => x.MaTuyen == tuyen.TuyenCha).ToList();
                            if (chuyenCha.Any())
                            {
                                var chuyenTauCha = chuyenTauAll.FirstOrDefault(x => chuyenCha.Any(y => y.MaChuyen == x.MaChuyen) && x.MaTau == chuyenTau.MaTau);
                                if (chuyenTauCha != null)
                                {
                                    var loaiVesCha = loaiVes.Where(x => chuyenTauCha.MaChuyenTau == x.MaChuyenTau).ToList();
                                    var veChas = ves.Where(x => loaiVesCha.Any(y => y.MaLoaiVe == x.MaLoaiVe) && (x.TrangThai == (int)TrangThaiVeEnum.DaThanhToan || x.TrangThai == (int)TrangThaiVeEnum.ChuaThanhToan)).ToList();
                                    if (veChas.Any())
                                    {
                                        chuyenTau.SoGheDaDat += veChas.Count;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Kiểm tra tuyến con
                            var tuyenCon = tuyens.FirstOrDefault(x => x.TuyenCha == tuyen.MaTuyen);
                            if (tuyenCon != null)
                            {
                                var chuyenCons = chuyens.Where(x => x.MaTuyen == tuyenCon.MaTuyen).ToList();
                                if (chuyenCons.Any())
                                {
                                    var chuyenTauCon = chuyenTauAll.FirstOrDefault(x => chuyenCons.Any(y => y.MaChuyen == x.MaChuyen) && x.MaTau == chuyenTau.MaTau);
                                    if (chuyenTauCon != null)
                                    {
                                        var loaiVeTauCon = loaiVes.Where(x => x.MaChuyenTau == chuyenTauCon.MaChuyenTau).ToList();
                                        var veCons = ves.Where(x => loaiVeTauCon.Any(y => y.MaLoaiVe == x.MaLoaiVe) && (x.TrangThai == (int)TrangThaiVeEnum.DaThanhToan || x.TrangThai == (int)TrangThaiVeEnum.ChuaThanhToan)).ToList();
                                        if (veCons.Any())
                                        {
                                            chuyenTau.SoGheDaDat += veCons.Count;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


                return new ResponseModel()
                {
                    Status = true,
                    Data = lstChuyenTau
                };
            }
        }

        [Route("them-chuyen-tau")]
        [HttpPost]
        public ResponseModel ThemChuyenTau(ChuyenTauThemModel model)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var tau = db.Taus.FirstOrDefault(x => x.MaTau == model.MaTau);
                if (tau == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Tàu không tồn tại"
                    };
                }

                var chuyen = db.Chuyens.FirstOrDefault(x => x.MaChuyen == model.MaChuyen);
                if (chuyen == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Chuyến không tồn tại"
                    };
                }

                if (model.VeNgoi <= 0 || model.VeNam <= 0)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Giá vé không hợp lệ"
                    };
                }

                //Kiem tra chuyen tau da ton tai chua
                var chuyenTauExited = db.ChuyenTaus.FirstOrDefault(x => x.MaChuyen == chuyen.MaChuyen && x.MaTau == tau.MaTau && x.NgayKhoiHanh == model.NgayKhoiHanh);
                if (chuyenTauExited != null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Đã có chuyến tàu vào ngày này rồi"
                    };
                }

                var newChuyenTau = new ChuyenTau()
                {
                    MaChuyen = chuyen.MaChuyen,
                    MaTau = tau.MaTau,
                    NgayKhoiHanh = model.NgayKhoiHanh,
                    CreatedByUser = TaiKhoanDangNhap.TaiKhoan,
                    DateCreated = DateTime.Now,
                    TrangThai = (int)TrangThaiChuyenTauEnum.ChuaKhoiHanh
                };

                db.ChuyenTaus.InsertOnSubmit(newChuyenTau);
                db.SubmitChanges();

                var newVeNgoi = new LoaiVe()
                {
                    MaChuyenTau = newChuyenTau.MaChuyenTau,
                    LoaiVe1 = (int)LoaiToaTauEnum.ToaNgoi,
                    GiaVe = model.VeNgoi
                };

                var newVeNam = new LoaiVe()
                {
                    MaChuyenTau = newChuyenTau.MaChuyenTau,
                    LoaiVe1 = (int)LoaiToaTauEnum.ToaNam,
                    GiaVe = model.VeNam
                };

                var listNewLoaiVe = new List<LoaiVe>();
                listNewLoaiVe.Add(newVeNgoi);
                listNewLoaiVe.Add(newVeNam);

                db.LoaiVes.InsertAllOnSubmit(listNewLoaiVe);
                db.SubmitChanges();

                return new ResponseModel()
                {
                    Status = true,
                    Message = "Thêm chuyến tàu thành công"
                };
            }
        }

        [Route("xoa-chuyen-tau/{maChuyenTau}")]
        [HttpPost]
        public ResponseModel XoaChuyenTau(int maChuyenTau)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var chuyenTau = db.ChuyenTaus.FirstOrDefault(x => x.MaChuyenTau == maChuyenTau);
                if (chuyenTau == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Chuyến tàu không tồn tại"
                    };
                }

                //Kiểm tra chuyến tàu đã bán vé chưa, nếu đã bán vé rồi, không thể xóa
                var loaiVes = db.LoaiVes.Where(x => x.MaChuyenTau == chuyenTau.MaChuyenTau).ToList();
                var allVes = db.Ves.ToList();
                if (allVes.Any(x => loaiVes.Any(y => y.MaLoaiVe == x.MaLoaiVe)))
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Đã bán vé cho chuyến tàu này, không thể xóa"
                    };
                }

                db.LoaiVes.DeleteAllOnSubmit(loaiVes);
                db.ChuyenTaus.DeleteOnSubmit(chuyenTau);
                db.SubmitChanges();

                return new ResponseModel()
                {
                    Status = true,
                    Message = "Xóa thành công"
                };
            }
        }

        [Route("cap-nhat-trang-thai")]
        [HttpPost]
        public ResponseModel CapNhatTrangThai(ChuyenTauTrangThaiModel model)
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

                chuyenTau.TrangThai = (int)model.TrangThai;
                chuyenTau.UpdatedByUser = TaiKhoanDangNhap.TaiKhoan;
                chuyenTau.DateUpdated = DateTime.Now;
                db.SubmitChanges();
                return new ResponseModel()
                {
                    Status = true
                };
            }
        }
    }
}
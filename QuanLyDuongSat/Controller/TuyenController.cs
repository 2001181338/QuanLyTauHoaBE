using QuanLyDuongSat.Enumeration;
using QuanLyDuongSat.GlobalVariable;
using QuanLyDuongSat.Model.ChuyenTauModel;
using QuanLyDuongSat.Model.ResponseModel;
using QuanLyDuongSat.Model.ToaModel;
using QuanLyDuongSat.Model.TuyenModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QuanLyDuongSat.Controller
{
    [RoutePrefix("api/tuyen")]
    public class TuyenController : ApiController
    {
        [Route("get-all")]
        [HttpGet]
        public ResponseModel GetAll()
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var chuyens = db.Chuyens.ToList();
                var tuyens = from tuyen in db.Tuyens
                             join gaDi in db.Gas on tuyen.GaDi equals gaDi.MaGa
                             join gaDen in db.Gas on tuyen.GaDen equals gaDen.MaGa
                             join tinhGadi in db.ThanhPho_Tinhs on gaDi.MaThanhPhoTinh equals tinhGadi.MaThanhPhoTinh
                             join tinhGaden in db.ThanhPho_Tinhs on gaDen.MaThanhPhoTinh equals tinhGaden.MaThanhPhoTinh
                             select new TuyenGetAllModel
                             {
                                 MaTuyen = tuyen.MaTuyen,
                                 MaGaDi = gaDi.MaGa,
                                 MaGaDen = gaDen.MaGa,
                                 TenGaDi = gaDi.TenGa,
                                 TenGaDen = gaDen.TenGa,
                                 MaTuyenCha = tuyen.TuyenCha,
                                 TenTinhGaDi = tinhGadi.TenThanhPhoTinh,
                                 TenTinhGaDen = tinhGaden.TenThanhPhoTinh
                             };
                var lstTuyen = tuyens.ToList();
                var allTuyen = db.Tuyens.ToList();
                var allGa = db.Gas.ToList();

                foreach(var tuyen in lstTuyen)
                {
                    tuyen.NotAllowEdit = chuyens.Any(x => x.MaTuyen == tuyen.MaTuyen);
                    if(tuyen.MaTuyenCha != null)
                    {
                        var tuyenCha = allTuyen.FirstOrDefault(x => x.MaTuyen == tuyen.MaTuyenCha);
                        if(tuyenCha != null)
                        {
                            var gaDi = allGa.FirstOrDefault(x => x.MaGa == tuyenCha.GaDi);
                            var gaDen = allGa.FirstOrDefault(x => x.MaGa == tuyenCha.GaDen);
                            tuyen.TenGaChaDi = gaDi.TenGa;
                            tuyen.TenGaChaDen = gaDen.TenGa;
                        }
                    }
                    
                }
                return new ResponseModel()
                {
                    Data = lstTuyen.ToList(),
                    Status = true
                };
            }
        }

        [Route("them-tuyen")]
        [HttpPost]
        public ResponseModel ThemTuyen(TuyenThemModel model)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var gaDi = db.Gas.FirstOrDefault(x => x.MaGa == model.MaGaDi);
                var gaDen = db.Gas.FirstOrDefault(x => x.MaGa == model.MaGaDen);
                var tuyenCha = db.Tuyens.FirstOrDefault(x => x.MaTuyen == model.MaTuyenCha);
                var allTuyen = db.Tuyens.ToList();

                if (gaDi == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Ga đi không tồn tại"
                    };
                }

                if (gaDen == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Ga đến không tồn tại"
                    };
                }

                if (model.MaTuyenCha != null && tuyenCha == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Tuyến cha không tồn tại"
                    };
                }

                var tuyen = db.Tuyens.FirstOrDefault(x => x.GaDi == model.MaGaDi && x.GaDen == model.MaGaDen);
                if (tuyen != null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Tuyến này đã tồn tại"
                    };
                }

                var tuyenChaUsed = allTuyen.FirstOrDefault(x => x.TuyenCha == model.MaTuyenCha);
                if(model.MaTuyenCha != null && tuyenChaUsed != null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Không được trùng tuyến cha"
                    };
                }

                var newTuyen = new Tuyen()
                {
                    GaDi = model.MaGaDi,
                    GaDen = model.MaGaDen,
                    TuyenCha = model.MaTuyenCha,
                    CreatedByUser = TaiKhoanDangNhap.TaiKhoan,
                    DateCreated = DateTime.Now
                };

                db.Tuyens.InsertOnSubmit(newTuyen);
                db.SubmitChanges();

                return new ResponseModel()
                {
                    Status = true,
                    Message = "Thêm thành công"
                };
            }
        }

        [Route("sua-tuyen")]
        [HttpPost]
        public ResponseModel SuaTuyen(TuyenSuaModel model)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var gaDi = db.Gas.FirstOrDefault(x => x.MaGa == model.MaGaDi);
                var gaDen = db.Gas.FirstOrDefault(x => x.MaGa == model.MaGaDen);
                var tuyenCha = db.Tuyens.FirstOrDefault(x => x.MaTuyen == model.MaTuyenCha);

                if (gaDi == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Ga đi không tồn tại"
                    };
                }

                if (gaDen == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Ga đến không tồn tại"
                    };
                }

                if (model.MaTuyenCha != null && tuyenCha == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Tuyến cha không tồn tại"
                    };
                }

                var currentTuyen = db.Tuyens.FirstOrDefault(x => x.MaTuyen == model.MaTuyen);
                if (currentTuyen == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Tuyến này không tồn tại"
                    };
                }

                var chuyen = db.Chuyens.FirstOrDefault(x => x.MaTuyen == currentTuyen.MaTuyen);
                if(chuyen != null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Tuyến này đang được sử dụng bởi Chuyến"
                    };
                }

                var tuyen = db.Tuyens.FirstOrDefault(x => x.GaDi == model.MaGaDi && x.GaDen == model.MaGaDen);
                if (tuyen != null && tuyen.MaTuyen != model.MaTuyen)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Tuyến này đã tồn tại"
                    };
                }

                var allTuyen = db.Tuyens.ToList();
                var tuyenChaUsed = allTuyen.FirstOrDefault(x => x.TuyenCha == model.MaTuyenCha);
                if (model.MaTuyenCha != null && tuyenChaUsed != null && tuyenChaUsed.MaTuyen != currentTuyen.MaTuyen)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Không được trùng tuyến cha"
                    };
                }

                currentTuyen.GaDi = model.MaGaDi;
                currentTuyen.GaDen = model.MaGaDen;
                currentTuyen.TuyenCha = model.MaTuyenCha;
                currentTuyen.UpdatedByUser = TaiKhoanDangNhap.TaiKhoan;
                currentTuyen.DateUpdated = DateTime.Now;

                db.SubmitChanges();

                return new ResponseModel()
                {
                    Status = true,
                    Message = "Sửa thành công",
                    Data = new TuyenGetAllModel()
                    {
                        MaTuyen = currentTuyen.MaTuyen,
                        MaGaDi = gaDi.MaGa,
                        TenGaDi = gaDi.TenGa,
                        MaGaDen = gaDen.MaGa,
                        TenGaDen = gaDen.TenGa,
                        MaTuyenCha = currentTuyen.TuyenCha
                    }
                };
            }
        }

        [Route("xoa-tuyen")]
        [HttpPost]
        public ResponseModel XoaTuyen(int id)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var tuyen = db.Tuyens.FirstOrDefault(x => x.MaTuyen == id);
                if(tuyen == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Tuyến này không tồn tại"
                    };
                }

                //Kiem tra tuyen nay co dang duoc su dung khong
                var chuyens = db.Chuyens.FirstOrDefault(x => x.MaTuyen == tuyen.MaTuyen);
                if(chuyens != null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Tuyến này đang được thiết lập trên Chuyến"
                    };
                }

                db.Tuyens.DeleteOnSubmit(tuyen);
                db.SubmitChanges();

                return new ResponseModel()
                {
                    Status = true,
                    Message = "Xóa thành công"
                };
            }
        }

        [Route("tim-tuyen")]
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
                var danhSachToa = db.Toas.ToList();

                var listChuyenTau = new List<ChuyenTauModel>();

                var loaiVes = db.LoaiVes.ToList();
             

                foreach (var chuyenTau in chuyenTaus)
                {
                    var chuyen = chuyens.FirstOrDefault(x => x.MaChuyen == chuyenTau.MaChuyen);
                    var sNgayKhoiHanh = chuyenTau.NgayKhoiHanh.ToString().Substring(0, 10) + " " + chuyen.GioKhoiHanh;
                    var ngayKhoiHanh = DateTime.Parse(sNgayKhoiHanh);
                    var gioConvertSeconds = (ngayKhoiHanh - DateTime.Now).TotalSeconds;

                    var chuyenTauModel = new ChuyenTauModel()
                    {
                        MaChuyenTau = chuyenTau.MaChuyenTau,
                        GaDi = gaDi.TenGa,
                        GaDen = gaDen.TenGa,
                        GioKhoiHanh = chuyen?.GioKhoiHanh,
                        NgayKhoiHanh = chuyenTau?.NgayKhoiHanh.ToString().Substring(0, 10),
                        TenTau = allTau.FirstOrDefault(x => x.MaTau == chuyenTau.MaTau)?.TenTau,
                        GiaVeNgoi = loaiVes.FirstOrDefault(x => x.MaChuyenTau == chuyenTau.MaChuyenTau && x.LoaiVe1 == 2).GiaVe ?? 0,
                        GiaVeNam = loaiVes.FirstOrDefault(x => x.MaChuyenTau == chuyenTau.MaChuyenTau && x.LoaiVe1 == 1).GiaVe ?? 0,
                        Toas = danhSachToa.Where(x => x.MaTau == chuyenTau.MaTau).Select(y => new ToaTimChuyenTauModel()
                        {
                            MaToa = y.MaToa,
                            TenToa = y.TenToa,
                            LoaiToa = (LoaiToaTauEnum)y.LoaiCho
                        }).OrderBy(z => z.TenToa).ToList(),
                        HetHan = gioConvertSeconds < 86400,
                        TrangThai = (TrangThaiChuyenTauEnum) chuyenTau.TrangThai
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
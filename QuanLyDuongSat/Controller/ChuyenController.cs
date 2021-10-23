using QuanLyDuongSat.Model.ChuyenModel;
using QuanLyDuongSat.Model.ResponseModel;
using QuanLyDuongSat.Model.TuyenModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QuanLyDuongSat.Controller
{
    [RoutePrefix("api/chuyen")]
    public class ChuyenController : ApiController
    {
        [Route("get-all")]
        [HttpGet]
        public ResponseModel GetAll()
        {
            using(QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var chuyenTau = db.ChuyenTaus.ToList();
                var res = from chuyen in db.Chuyens
                          join tuyen in db.Tuyens on chuyen.MaTuyen equals tuyen.MaTuyen
                          join GaDi in db.Gas on tuyen.GaDi equals GaDi.MaGa
                          join GaDen in db.Gas on tuyen.GaDen equals GaDen.MaGa
                          join tinhGaDi in db.ThanhPho_Tinhs on GaDi.MaThanhPhoTinh equals tinhGaDi.MaThanhPhoTinh
                          join tinhGaDen in db.ThanhPho_Tinhs on GaDen.MaThanhPhoTinh equals tinhGaDen.MaThanhPhoTinh
                          select new ChuyenGetAllModel
                          {
                              MaChuyen = chuyen.MaChuyen,
                              MaTuyen = tuyen.MaTuyen,
                              GioKhoiHanh = chuyen.GioKhoiHanh,
                              MaGaDi = GaDi.MaGa,
                              MaGaDen = GaDen.MaGa,
                              TenGaDi = GaDi.TenGa,
                              TenGaDen = GaDen.TenGa,
                              TinhGaDi = tinhGaDi.TenThanhPhoTinh,
                              TinhGaDen = tinhGaDen.TenThanhPhoTinh,
                              TimeOrderBy = TimeSpan.Parse(chuyen.GioKhoiHanh).TotalHours
                          };
                var lstChuyen = res.ToList();
                foreach(var chuyen in lstChuyen)
                {
                    chuyen.NotAllowEdit = chuyenTau.Any(x => x.MaChuyen == chuyen.MaChuyen);
                }
                return new ResponseModel()
                {
                    Status = true,
                    Data = lstChuyen.ToList()
                };
            }
        }

        [Route("them-chuyen")]
        [HttpPost]
        public ResponseModel ThemChuyen(ChuyenThemModel model)
        {
            using(QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var tuyenCheck = db.Tuyens.FirstOrDefault(x => x.MaTuyen == model.MaTuyen);
                if(tuyenCheck == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Tuyến này không tồn tại"
                    };
                }

                var chuyenExisted = db.Chuyens.FirstOrDefault(x => x.MaTuyen == model.MaTuyen && x.GioKhoiHanh == model.GioKhoiHanh);
                if(chuyenExisted != null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Chuyến này khởi hành giờ này đã tồn tại"
                    };
                }

                var newChuyen = new Chuyen()
                {
                    MaTuyen = model.MaTuyen,
                    GioKhoiHanh = model.GioKhoiHanh
                };

                db.Chuyens.InsertOnSubmit(newChuyen);
                db.SubmitChanges();

                return new ResponseModel()
                {
                    Status = true,
                    Message = "Thêm thành công"
                };
            }
        }

        [Route("sua-chuyen")]
        [HttpPost]
        public ResponseModel SuaChuyen(SuaChuyenModel model)
        {
            using(QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var chuyen = db.Chuyens.FirstOrDefault(x => x.MaChuyen == model.MaChuyen);
                if(chuyen == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Chuyến này không tồn tại"
                    };
                }

                var chuyenTau = db.ChuyenTaus.FirstOrDefault(x => x.MaChuyen == chuyen.MaChuyen);
                if(chuyenTau != null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Chuyến này đang được thiết lập trên chuyến tàu"
                    };
                }

                var chuyenExisted = db.Chuyens.FirstOrDefault(x => x.MaTuyen == model.MaTuyen && x.GioKhoiHanh == model.GioKhoiHanh);
                if(chuyenExisted != null && chuyenExisted.MaChuyen != chuyen.MaChuyen)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Chuyến này đã tồn tại"
                    };
                }

                chuyen.MaTuyen = model.MaTuyen;
                chuyen.GioKhoiHanh = model.GioKhoiHanh;

                db.SubmitChanges();

                return new ResponseModel()
                {
                    Status = true,
                    Message = "Sửa thành công"
                };
            }
        }
    
        [Route("xoa-chuyen")]
        [HttpPost]
        public ResponseModel Xoachuyen(int id)
        {
            using(QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var chuyen = db.Chuyens.FirstOrDefault(x => x.MaChuyen == id);
                if(chuyen == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Chuyến này không tồn tại"
                    };
                }

                //Kiểm tra chuyến này đã được thiết lập trên chuyến tàu chưa?
                var chuyenTau = db.ChuyenTaus.FirstOrDefault(x => x.MaChuyen == chuyen.MaChuyen);
                if(chuyenTau != null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Chuyến này đang được thiết lập trên chuyến tàu"
                    };
                }

                db.Chuyens.DeleteOnSubmit(chuyen);
                db.SubmitChanges();

                return new ResponseModel()
                {
                    Status = true,
                    Message = "Xóa thành công"
                };
            }
        }
    }
}

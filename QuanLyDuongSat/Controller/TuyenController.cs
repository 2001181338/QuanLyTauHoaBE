﻿using QuanLyDuongSat.Model.ResponseModel;
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
                return new ResponseModel()
                {
                    Data = tuyens.ToList(),
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

                var newTuyen = new Tuyen()
                {
                    GaDi = model.MaGaDi,
                    GaDen = model.MaGaDen,
                    TuyenCha = model.MaTuyenCha
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

                var tuyen = db.Tuyens.FirstOrDefault(x => x.GaDi == model.MaGaDi && x.GaDen == model.MaGaDen);
                if (tuyen != null && tuyen.MaTuyen != model.MaTuyen)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Tuyến này đã tồn tại"
                    };
                }

                currentTuyen.GaDi = model.MaGaDi;
                currentTuyen.GaDen = model.MaGaDen;
                currentTuyen.TuyenCha = model.MaTuyenCha;

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
    }
}
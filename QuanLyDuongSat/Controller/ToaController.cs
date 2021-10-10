using QuanLyDuongSat.Enumeration;
using QuanLyDuongSat.Model.ResponseModel;
using QuanLyDuongSat.Model.ToaModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QuanLyDuongSat.Controller
{
    [RoutePrefix("api/toa")]
    public class ToaController : ApiController
    {
        [Route("get-all")]
        [HttpGet]
        public ResponseModel GetAll()
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var toas = db.Toas.ToList();
                var taus = db.Taus.ToList();
                var ghes = db.Ghes.ToList();

                var res = toas.Select(x => new ToaGetAllModel()
                {
                    MaToa = x.MaToa,
                    TenToa = x.TenToa,
                    LoaiCho = (LoaiToaTauEnum)x.LoaiCho,
                    MaTau = taus.FirstOrDefault(y => y.MaTau == x.MaTau).MaTau,
                    TenTau = taus.FirstOrDefault(y => y.MaTau == x.MaTau).TenTau,
                    SoLuongGhe = ghes.Where(y => y.MaToa == x.MaToa).Count()
                }).ToList();

                return new ResponseModel()
                {
                    Status = true,
                    Data = res
                };
            }
        }

        [Route("them-toa")]
        [HttpPost]
        public ResponseModel ThemToa(ThemToaModel model)
        {
            if (model.SoLuongGhe <= 0)
            {
                return new ResponseModel()
                {
                    Status = false,
                    Message = "Số lượng ghế phải lớn hơn 0"
                };
            }

            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var tauExisted = db.Taus.FirstOrDefault(x => x.MaTau == model.MaTau);
                if (tauExisted == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Tàu này không tồn tại"
                    };
                }

                var tenToa = model.TenToa.ToUpper().Trim();

                //Kiểm tra tên toa đã tồn tại chưa
                var toaExisted = db.Toas.FirstOrDefault(x => x.MaTau == model.MaTau && x.TenToa.ToUpper().Trim() == tenToa);
                if (toaExisted != null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Tên toa trên tàu này đã tồn tại"
                    };
                }

                var newToa = new Toa()
                {
                    TenToa = model.TenToa.ToUpper().Trim(),
                    MaTau = model.MaTau,
                    LoaiCho = (int)model.LoaiToa
                };

                db.Toas.InsertOnSubmit(newToa);
                db.SubmitChanges();

                //Khởi tạo ghế
                var newGhes = new List<Ghe>();
                for (int i = 0; i < model.SoLuongGhe; i++)
                {
                    var newGhe = new Ghe()
                    {
                        TenGhe = tenToa + i,
                        MaToa = newToa.MaToa,
                        DaXoa = false
                    };

                    newGhes.Add(newGhe);
                }

                db.Ghes.InsertAllOnSubmit(newGhes);
                db.SubmitChanges();

                return new ResponseModel()
                {
                    Status = true,
                    Message = "Thêm thành công"
                };
            }
        }

        [Route("xoa-toa")]
        [HttpPost]
        public ResponseModel XoaToa(int id)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var toaExisted = db.Toas.FirstOrDefault(x => x.MaToa == id);
                if (toaExisted == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Toa này không tồn tại"
                    };
                }

                var ghes = db.Ghes.Where(x => x.MaToa == id).ToList();
                if (ghes.Any())
                {
                    db.Ghes.DeleteAllOnSubmit(ghes);
                }

                db.Toas.DeleteOnSubmit(toaExisted);
                db.SubmitChanges();

                return new ResponseModel()
                {
                    Status = true,
                    Message = "Xóa thành công"
                };
            }
        }

        [Route("sua-toa")]
        [HttpPost]
        public ResponseModel SuaToa(SuaToaModel model)
        {
            if (model.SoLuongGhe <= 0)
            {
                return new ResponseModel()
                {
                    Status = false,
                    Message = "Số lượng ghế phải lớn hơn 0"
                };
            }
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var tauExisted = db.Taus.FirstOrDefault(x => x.MaTau == model.MaTau);
                if (tauExisted == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Tàu này không tồn tại"
                    };
                }

                var currentToa = db.Toas.FirstOrDefault(x => x.MaToa == model.MaToa);
                if (currentToa == null)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Toa này không tồn tại"
                    };
                }

                //Kiểm tra tên toa đã tồn tại chưa
                var tenToa = model.TenToa.ToUpper().Trim();

                var toaExisted = db.Toas.FirstOrDefault(x => x.MaTau == model.MaTau && x.TenToa.ToUpper().Trim() == tenToa);
                if (toaExisted != null && toaExisted.MaToa != currentToa.MaToa)
                {
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Tên toa trên tàu này đã tồn tại"
                    };
                }

                //Update data
                currentToa.TenToa = tenToa;
                currentToa.MaTau = model.MaTau;
                currentToa.LoaiCho = (int)model.LoaiToa;

                var ghes = db.Ghes.Where(x => x.MaToa == currentToa.MaToa).ToList();
                //Xoa ghe nếu SoLuongGheEdit > SoLuongGheHienTai
                if (ghes.Count > model.SoLuongGhe)
                {
                    //Kiểm tra xem ghế này có đang được đặt hay không. 
                    //Đã đặt -> báo lỗi
                    //Chưa đặt -> xóa ghế
                    return new ResponseModel()
                    {
                        Status = false,
                        Message = "Đã có ghế đang đặt"
                    };

                }
                else if (ghes.Count < model.SoLuongGhe)
                {
                    var maGheMax = ghes.Max(x => x.MaGhe);
                    var gheCuoi = ghes.FirstOrDefault(x => x.MaGhe == maGheMax);
                    if (gheCuoi != null)
                    {
                        int soGheMoi = model.SoLuongGhe - ghes.Count;
                        var orderBy = gheCuoi.TenGhe.Substring(1);
                        var newGhes = new List<Ghe>();
                        for (int i = int.Parse(orderBy) + 1; i <= soGheMoi; i++)
                        {
                            var newGhe = new Ghe()
                            {
                                TenGhe = tenToa + i,
                                MaToa = currentToa.MaToa,
                                DaXoa = false
                            };
                            newGhes.Add(newGhe);
                        }

                        db.Ghes.InsertAllOnSubmit(newGhes);
                    }
                }

                db.SubmitChanges();
                return new ResponseModel()
                {
                    Status = true,
                    Message = "Sửa thành công"
                };
            }
        }
    }
}

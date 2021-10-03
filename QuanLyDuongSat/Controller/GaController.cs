using QuanLyDuongSat.Model.GaModel;
using QuanLyDuongSat.Model.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QuanLyDuongSat.Controller
{
    public class GaController : ApiController
    {
        [Route("api/ga/get-all")]
        [HttpGet]
        public ResponseModel GetAll()
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var dsGa = from ga in db.Gas
                           join tinh in db.ThanhPho_Tinhs on ga.MaThanhPhoTinh equals tinh.MaThanhPhoTinh
                           select new GaModel
                           {
                               MaGa = ga.MaGa,
                               TenGa = ga.TenGa,
                               MaThanhPhoTinh = tinh.MaThanhPhoTinh,
                               TenThanhPhoTinh = tinh.TenThanhPhoTinh
                           };
                return new ResponseModel()
                {
                    Data = dsGa.OrderBy(x => x.TenGa).ToList(),
                    Status = true
                };
            }
        }

        [Route("api/ga/them-ga")]
        [HttpPost]
        public ResponseModel ThemGa(GaThemRequestModel model)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var gaExisted = db.Gas.FirstOrDefault(x => x.TenGa.ToLower().Trim() == model.TenGa.ToLower().Trim() && x.MaThanhPhoTinh == model.MaThanhPhoTinh);
                if (gaExisted != null)
                {
                    return new ResponseModel()
                    {
                        Data = gaExisted.TenGa,
                        Status = false,
                        Message = "Ga đã tồn tại"
                    };
                }

                var tinhThanhPho = db.ThanhPho_Tinhs.FirstOrDefault(x => x.MaThanhPhoTinh == model.MaThanhPhoTinh);

                var newGa = new Ga()
                {
                    TenGa = model.TenGa,
                    MaThanhPhoTinh = model.MaThanhPhoTinh
                };

                db.Gas.InsertOnSubmit(newGa);
                db.SubmitChanges();

                var resData = new GaModel()
                {
                    MaGa = newGa.MaGa,
                    TenGa = newGa.TenGa,
                    MaThanhPhoTinh = model.MaThanhPhoTinh,
                    TenThanhPhoTinh = tinhThanhPho.TenThanhPhoTinh
                };

                return new ResponseModel()
                {
                    Data = resData,
                    Status = true,
                    Message = "Thêm thành công"
                };
            }
        }


        [Route("api/ga/sua-ga")]
        [HttpPut]
        public ResponseModel SuaGa([FromUri] int maGa, [FromBody] GaThemRequestModel model)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                //Check ga existed? false -> error
                var gaExisted = db.Gas.FirstOrDefault(x => x.MaGa == maGa);
                if (gaExisted == null)
                {
                    return new ResponseModel()
                    {
                        Data = gaExisted.TenGa,
                        Status = false,
                        Message = "Ga không tồn tại"
                    };
                }


                //Check tenGa existed? True -> error
                var gaTenExisted = db.Gas.FirstOrDefault(x => x.MaGa != maGa && x.TenGa.ToLower().Trim() == model.TenGa.ToLower().Trim() && x.MaThanhPhoTinh == model.MaThanhPhoTinh);

                if (gaExisted != null)
                {
                    return new ResponseModel()
                    {
                        Data = gaExisted.TenGa,
                        Status = false,
                        Message = "Ga đã tồn tại"
                    };
                }

                //Update data
                gaExisted.TenGa = model.TenGa;
                gaExisted.MaThanhPhoTinh = model.MaThanhPhoTinh;

                db.SubmitChanges();

                //response data for FE
                var tinhThanhPho = db.ThanhPho_Tinhs.FirstOrDefault(x => x.MaThanhPhoTinh == model.MaThanhPhoTinh);
                var resData = new GaModel()
                {
                    MaGa = gaExisted.MaGa,
                    TenGa = gaExisted.TenGa,
                    MaThanhPhoTinh = tinhThanhPho.MaThanhPhoTinh,
                    TenThanhPhoTinh = tinhThanhPho.TenThanhPhoTinh
                };

                return new ResponseModel()
                {
                    Data = resData,
                    Status = true,
                    Message = "Sửa thành công"
                };
            }
        }
    }
}

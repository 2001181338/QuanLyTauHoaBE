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
    }
}

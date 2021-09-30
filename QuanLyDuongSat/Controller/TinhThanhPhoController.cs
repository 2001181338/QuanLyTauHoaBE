using QuanLyDuongSat.Model.ResponseModel;
using QuanLyDuongSat.Model.TinhThanhPhoModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QuanLyDuongSat.Controller
{
    public class TinhThanhPhoController : ApiController
    {
        [Route("api/tinhthanhpho/get-all")]
        [HttpGet]
        public ResponseModel GetAll()
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var dsTinhThanhPho = db.ThanhPho_Tinhs.ToList().Select(x => new TinhThanhPhoModel()
                {
                    MaTinhThanhPho = x.MaThanhPhoTinh,
                    TenTinhThanhPho = x.TenThanhPhoTinh
                }).ToList();
                return new ResponseModel()
                {
                    Data = dsTinhThanhPho.OrderBy(x => x.TenTinhThanhPho).ToList(),
                    Status = true
                };
            }
        }
    }
}

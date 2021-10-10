using QuanLyDuongSat.Enumeration;
using QuanLyDuongSat.Model.ResponseModel;
using QuanLyDuongSat.Model.TauModel;
using QuanLyDuongSat.Model.ToaModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QuanLyDuongSat.Controller
{
    [RoutePrefix("api/tau")]
    public class TauController : ApiController
    {
        [Route("get-all")]
        [HttpGet]
        public ResponseModel GetAll()
        {
            using(QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var taus = db.Taus.Select(x => new TauGetAllModel()
                {
                    MaTau = x.MaTau,
                    TenTau = x.TenTau
                }).ToList();

                return new ResponseModel()
                {
                    Status = true,
                    Data = taus
                };
            }
        }
   
    }
}

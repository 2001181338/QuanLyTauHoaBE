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
    }
}

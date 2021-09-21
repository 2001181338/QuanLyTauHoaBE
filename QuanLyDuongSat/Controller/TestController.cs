using QuanLyDuongSat.Model.SinhVien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace QuanLyDuongSat.Controller
{
    public class TestController : ApiController
    {
        [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
        public IHttpActionResult Get()
        {
            var sv = new ThemSinhVienModel()
            {
                HoTen = "ABC",
                NamSinh = 2000
            };

            return Ok(sv);
        }

        [Route("api/test/them-sinh-vien")]
        [HttpPost]
        public string PostSinhVien([FromBody] ThemSinhVienModel sinhVien)
        {
            return "Hello World";
        }

        [Route("api/test/them-giao-vien")]
        [HttpPost]
        public string PostGiaoVien([FromBody] ThemSinhVienModel sinhVien)
        {
            return "Hello World";
        }

    }
}

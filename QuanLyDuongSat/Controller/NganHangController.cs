using QuanLyDuongSat.Enumeration;
using QuanLyDuongSat.GlobalVariable;
using QuanLyDuongSat.Model.NganHangModel;
using QuanLyDuongSat.Model.NhanVienModel;
using QuanLyDuongSat.Model.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace QuanLyDuongSat.Controller
{
    [RoutePrefix("api/nganhang")]
    public class NganHangController : ApiController
    {
        [Route("dangnhap")]
        [HttpPost]
        public ResponseModel DangNhap(NganHangDangNhapModel model)
        {
            if (string.IsNullOrEmpty(model.CMND) || string.IsNullOrEmpty(model.MaBaoMat))
            {
                return new ResponseModel
                {
                    Data = null,
                    Message = "Vui lòng không để trống",
                    Status = false
                };
            }
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var nganHang = db.NganHangs.FirstOrDefault(x => x.CMND == model.CMND && x.MaBaoMat == model.MaBaoMat);
                if (nganHang == null)
                {
                    return new ResponseModel
                    {
                        Data = null,
                        Message = "CMND/CCCD hoặc mã bảo mật không đúng",
                        Status = false
                    };
                }

                return new ResponseModel
                {
                    Data = model,
                    Message = "Kết nối thành công",
                    Status = true
                };
            }
        }

    }
}

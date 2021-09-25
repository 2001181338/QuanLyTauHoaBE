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
    public class NhanVienController : ApiController
    {
        [Route("api/nhanvien/dangnhap")]
        [HttpPost]
        public ResponseModel DangNhap(NhanVienDangNhapModel model)
        {
            using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var nhanVienCheck = db.QuanTris.FirstOrDefault(x => x.TaiKhoan.ToLower().Trim() == model.TaiKhoan.ToLower().Trim() && x.MatKhau.ToLower().Trim() == model.MatKhau.ToLower().Trim());
                if (nhanVienCheck == null)
                {
                    return new ResponseModel
                    {
                        Data = null,
                        Message = "Tên đăng nhập hoặc mật khẩu không đúng",
                        Status = false
                    };
                }

                return new ResponseModel
                {
                    Data = model,
                    Message = "Đăng nhập thành công",
                    Status = true
                };
            }
        }
    }
}

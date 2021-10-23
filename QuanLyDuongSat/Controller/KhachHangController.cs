using QuanLyDuongSat.Model.KhachHangModel;
using QuanLyDuongSat.Model.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QuanLyDuongSat.Controller
{
    [RoutePrefix("api/khachhang")]
    public class KhachHangController : ApiController
    {
        //[Route("dangnhap")]
        //[HttpPost]
        //public ResponseModel DangNhap(KhachHangModel model)
        //{
        //    if (string.IsNullOrEmpty(model.SoDT) || string.IsNullOrEmpty(model.MatKhau))
        //    {
        //        return new ResponseModel
        //        {
        //            Data = null,
        //            Message = "Vui lòng không để trống!",
        //            Status = false
        //        };
        //    }
        //    using (QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
        //    {
        //        var khachHangKTTK = db.KhachHangs.FirstOrDefault(x => x.SoDT.ToLower().Trim() == model.SoDT.ToLower().Trim() && x.MatKhau.ToLower().Trim() == model.MatKhau.ToLower().Trim());
        //        if (khachHangKTTK == null)
        //        {
        //            return new ResponseModel
        //            {
        //                Data = null,
        //                Message = "Tên đăng nhập hoặc mật khẩu không đúng!",
        //                Status = false
        //            };
        //        }

        //        return new ResponseModel
        //        {
        //            Data = new KhachHangResponseModel()
        //            {
        //                HoTen = khachHangKTTK.HoTen,
        //                MaKhach = khachHangKTTK.MaKhach,
        //                SoDT = khachHangKTTK.SoDT
        //            },
        //            Message = "Đăng nhập thành công",
        //            Status = true
        //        };
        //    }
        //}
    }
}

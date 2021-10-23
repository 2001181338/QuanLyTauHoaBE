﻿using QuanLyDuongSat.Enumeration;
using QuanLyDuongSat.GlobalVariable;
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
    [RoutePrefix("api/nhanvien")]
    public class NhanVienController : ApiController
    {
        [Route("dangnhap")]
        [HttpPost]
        public ResponseModel DangNhap(NhanVienDangNhapModel model)
        {
            if(string.IsNullOrEmpty(model.TaiKhoan) || string.IsNullOrEmpty(model.MatKhau))
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

                TaiKhoanDangNhap.TaiKhoan = nhanVienCheck.TaiKhoan;

                return new ResponseModel
                {
                    Data = model,
                    Message = "Đăng nhập thành công",
                    Status = true
                };
            }
        }
      
        [Route("dashboard")]
        [HttpGet]
        public ResponseModel GetInfoDashBoard()
        {
            using(QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var veDaThanhToan = db.Ves.Where(x => x.TrangThai == (int)TrangThaiVeEnum.DaThanhToan).ToList();
                var VeDaTinhPhi = db.Ves.Where(x => x.PhiTraVe != null && x.PhiTraVe != 0).ToList();
                var loaiVes = db.LoaiVes.ToList();

                double doanhThu = 0;
                foreach (var ve in veDaThanhToan)
                {
                    var loaiVe = loaiVes.FirstOrDefault(x => x.MaLoaiVe == ve.MaLoaiVe);
                    if(loaiVe != null)
                    {
                        doanhThu += loaiVe.GiaVe ?? 0;
                    }
                }

                doanhThu += VeDaTinhPhi.Sum(x => x.PhiTraVe) ?? 0;
                
                var res = new NhanVienDashBoardModel()
                {
                    SoLuongTau = db.Taus.Count(),
                    TongSoGa = db.Gas.Count(),
                    TongSoKhachHang = db.KhachHangs.Count(),
                    TongDoanhThu = doanhThu
                };

                return new ResponseModel()
                {
                    Status = true,
                    Data = res
                };
            }
        }
    }   
}

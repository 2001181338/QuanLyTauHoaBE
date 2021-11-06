using FluentScheduler;
using QuanLyDuongSat.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyDuongSat.BackgroundJob
{
    public static class DailyTask
    {
        public static void XoaVeHetHan()
        {
            using(QuanLyDuongSatDBDataContext db = new QuanLyDuongSatDBDataContext())
            {
                var ves = db.Ves.Where(x => x.TrangThai == (int)TrangThaiVeEnum.ChuaThanhToan).ToList();
                var seconds = 3600;

                foreach (var ve in ves)
                {
                    var gioDaTru = (ve.NgayBanVe.Value - DateTime.Now).TotalSeconds;
                    if (gioDaTru >= 43200)
                    {
                        ve.TrangThai = (int)TrangThaiVeEnum.DaHuy;
                    }
                }

                db.SubmitChanges();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NDKFastfood.Models;
using PagedList;
using PagedList.Mvc;
namespace NDKFastfood.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        dbKiwiFastfoodDataContext data = new dbKiwiFastfoodDataContext(ConfigurationManager.ConnectionStrings["KiwiFastfoodConnectionString1"].ConnectionString);
        private List<MonAn> LayMonAn(int count)
        {
            return data.MonAns.OrderBy(a => a.MaMon).Take(count).ToList();
        }
        public ActionResult Index(int ? page)
        {
            int pageSize = 6;
            int pageNum = (page ?? 1);
            var monan = LayMonAn(20);
            return View(monan.ToPagedList(pageNum,pageSize));
        }

        public ActionResult ThucDon()
        {
            var thucdon = from td in data.Loais select td;
            return PartialView(thucdon);        
        }

        public ActionResult MATheothucdon(int id)
        {
            var monan = from ma in data.MonAns where ma.MaLoai == id select ma;
            return View(monan);
        }

        public ActionResult Details(int id)
        {
            var monan = from ma in data.MonAns where ma.MaMon == id select ma;
            return View(monan.Single());
        }

        public ActionResult XemDanhGia(int mamon)
        {
            var danhgia = data.DanhGias.Where(dg => dg.MaMon == mamon).OrderByDescending(dg => dg.NgayDanhGia).ToList();

            var danhGiaModel = danhgia.Select(dg => new XemDanhGia
            {
                HoTen = data.KhachHangs.FirstOrDefault(kh => kh.MaKH == dg.MaKH)?.HoTen,
                NoiDung = dg.NoiDung,
                NgayDanhGia = dg.NgayDanhGia
            }).ToList();

            return PartialView(danhGiaModel);  
        }

        [HttpPost]
        public ActionResult ThemDanhGia(int MaMon, string commentContent)
        {
            if (Session["TaiKhoan"] != null)
            {
                KhachHang kh = (KhachHang)Session["TaiKhoan"];
                int MaKH = kh.MaKH; 

                DanhGia danhGia = new DanhGia
                {
                    MaMon = MaMon,
                    MaKH = MaKH,
                    NoiDung = commentContent,
                    NgayDanhGia = DateTime.Now
                };

                data.DanhGias.InsertOnSubmit(danhGia);
                data.SubmitChanges();

                return RedirectToAction("Details", new { id = MaMon });
            }
            else
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
        }

    }
}
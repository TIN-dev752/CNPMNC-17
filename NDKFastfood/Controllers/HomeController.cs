using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NDKFastfood.Models;
using PagedList;

namespace NDKFastfood.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        dbKiwiFastfoodDataContext data = new dbKiwiFastfoodDataContext(ConfigurationManager.ConnectionStrings["KiwiFastfoodConnectionString1"].ConnectionString);
        private List<MonAn> LayMonAn(int count)
        {
            return data.MonAns.OrderByDescending(a => a.MaMon).Take(count).ToList();
        }
        public ActionResult Index()
        {
            var monan = LayMonAn(8);
            return View(monan);
        }
        public ActionResult ThucDon()
        {
            var thucdon = from td in data.Loais select td;
            return PartialView(thucdon);
        }
        public ActionResult Details(int id)
        {
            var monan = from ma in data.MonAns where ma.MaMon == id select ma;
            return View(monan.Single());
        }
        public ActionResult KhuyenMai(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            var khuyenMai = data.KhuyenMais
                         .Where(km => km.NgayKetThuc != null && km.NgayKetThuc.Date >= DateTime.Now.Date) 
                         .OrderBy(n => n.MaKM)
                         .ToList();
            return View(khuyenMai.ToPagedList(pageNumber, pageSize));
        }
    }
}
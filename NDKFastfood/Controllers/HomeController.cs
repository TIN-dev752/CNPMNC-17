using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NDKFastfood.Models;

namespace NDKFastfood.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        private dbKiwiFastfoodDataContext data;

        public HomeController()
        {
            // Lấy chuỗi kết nối từ Web.config
            string connectionString = ConfigurationManager.ConnectionStrings["KiwiFastfoodConnectionString"].ConnectionString;

            // Khởi tạo đối tượng dbKiwiFastfoodDataContext với chuỗi kết nối
            data = new dbKiwiFastfoodDataContext(connectionString);
        }
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
    }
}
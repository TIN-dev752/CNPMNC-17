using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NDKFastfood.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;
using System.Configuration;

namespace NDKFastfood.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        private dbKiwiFastfoodDataContext db;

        public AdminController()
        {
            // Lấy chuỗi kết nối từ Web.config
            string connectionString = ConfigurationManager.ConnectionStrings["KiwiFastfoodConnectionString"].ConnectionString;

            // Khởi tạo đối tượng dbKiwiFastfoodDataContext với chuỗi kết nối
            db = new dbKiwiFastfoodDataContext(connectionString);
        }
        public ActionResult Index()
        {
            return RedirectToAction("Login","Admin");
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                Admin ad = db.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    Session["TaiKhoanAdmin"] = ad;
                    return RedirectToAction("DonDatHang", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }
        public ActionResult MonAn(int ? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.MonAns.ToList().OrderBy(n => n.MaMon).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult ThemMonAn()
        {
            ViewBag.MaLoai = new SelectList(db.Loais.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemMonAn(MonAn monan, HttpPostedFileBase fileupload)
        {
            //đưa dữ liệu vào dropdownload
            ViewBag.MaLoai = new SelectList(db.Loais.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh đại diện";
                return View();
            }
            // thêm vào csdl
            else
            {
                if (ModelState.IsValid)
                {
                    //lưu tên file , lưu ý bổ sung thư việng using System.IO
                    var filename = Path.GetFileName(fileupload.FileName);
                    // lưu đường dẫn của file
                    var path = Path.Combine(Server.MapPath("~/Assets/Images/"),filename);
                    // kiểm tra hình tồn tại chưa?
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        // lưu hình ảnh vào  đường dẫn
                        fileupload.SaveAs(path);
                    }
                    monan.AnhDD = filename;
                    db.MonAns.InsertOnSubmit(monan);
                    db.SubmitChanges();
                }
                return RedirectToAction("MonAn");
            }
        }
        public ActionResult ChiTietMonAn(int id)
        {
            MonAn monan = db.MonAns.SingleOrDefault(n => n.MaMon == id);
            ViewBag.Mamon = monan.MaMon;
            if (monan == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(monan);
        }
        public ActionResult XoaMonAn(int id)
        {
            MonAn monan = db.MonAns.SingleOrDefault(n => n.MaMon == id);
            ViewBag.Mamon = monan.MaMon;
            if (monan == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(monan);
        }
        [HttpPost, ActionName("XoaMonAn")]
        public ActionResult XacNhanXoa(int id)
        {
            MonAn monan = db.MonAns.SingleOrDefault(n => n.MaMon == id);
            ViewBag.Mamon = monan.MaMon;
            if (monan == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.MonAns.DeleteOnSubmit(monan);
            db.SubmitChanges();
            return RedirectToAction("MonAn");
        }
        public ActionResult SuaMonAn(int id)
        {
            MonAn monan = db.MonAns.SingleOrDefault(n => n.MaMon == id);
            if (monan == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaLoai = new SelectList(db.Loais.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai", monan.MaLoai);
            return View(monan);
        }
        [HttpPost]
        public ActionResult SuaMonAn(MonAn monan)
        {
            MonAn itemm = db.MonAns.SingleOrDefault(n => n.MaMon == monan.MaMon);
            itemm.TenMon = monan.TenMon;
            itemm.GiaBan = monan.GiaBan;
            itemm.NoiDung = monan.NoiDung;
            itemm.SoLuongTon = monan.SoLuongTon;
            db.SubmitChanges();
            return RedirectToAction("MonAn");
        }
        public ActionResult Loai(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.Loais.ToList().OrderBy(n => n.MaLoai).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ThemLoai()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemLoai(Loai item)
        {
            db.Loais.InsertOnSubmit(item);
            db.SubmitChanges();
            return RedirectToAction("Loai");
        }
        public ActionResult SuaLoai(int id)
        {
            Loai item = db.Loais.SingleOrDefault(n => n.MaLoai == id);
            return View(item);
        }
        [HttpPost]
        public ActionResult SuaLoai(Loai loai)
        {
            Loai itemm = db.Loais.SingleOrDefault(n => n.MaLoai == loai.MaLoai);
            itemm.TenLoai = loai.TenLoai;
            db.SubmitChanges();
            return RedirectToAction("Loai");
        }
        public ActionResult ChiTietLoai(int id)
        {
            Loai item = db.Loais.SingleOrDefault(n => n.MaLoai == id);
            ViewBag.MaLoai = item.MaLoai;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        public ActionResult XoaLoai(int id)
        {
            Loai item = db.Loais.SingleOrDefault(n => n.MaLoai == id);
            ViewBag.MaLoai = item.MaLoai;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        [HttpPost, ActionName("XoaLoai")]
        public ActionResult XacNhanXoaLoai(int id)
        {
            Loai item = db.Loais.SingleOrDefault(n => n.MaLoai == id);
            ViewBag.MaLoai = item.MaLoai;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.Loais.DeleteOnSubmit(item);
            db.SubmitChanges();
            return RedirectToAction("Loai");
        }
        public ActionResult KhachHang(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.KhachHangs.ToList().OrderBy(n => n.MaKH).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult XoaKH(int id)
        {
            KhachHang item = db.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = item.MaKH;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        [HttpPost, ActionName("XoaKH")]
        public ActionResult XacNhanXoaKH(int id)
        {
            KhachHang item = db.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = item.MaKH;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.KhachHangs.DeleteOnSubmit(item);
            db.SubmitChanges();
            return RedirectToAction("KhachHang");
        }
        public ActionResult ChiTietKH(int id)
        {
            KhachHang item = db.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = item.MaKH;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        public ActionResult SuaKH(int id)
        {
            KhachHang item = db.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            return View(item);
        }
        [HttpPost]
        public ActionResult SuaKH(KhachHang kh)
        {
            KhachHang itemm = db.KhachHangs.SingleOrDefault(n => n.MaKH == kh.MaKH);
            itemm.HoTen = kh.HoTen;
            itemm.TaiKhoan = kh.TaiKhoan;
            itemm.MatKhau = kh.MatKhau;
            itemm.Email = kh.Email;
            itemm.DiaChiKH = kh.DiaChiKH;
            itemm.DienThoaiKH = kh.DienThoaiKH;
            itemm.NgaySinh = kh.NgaySinh;
            db.SubmitChanges();
            return RedirectToAction("KhachHang");
        }
        public ActionResult NhanVien(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.NhanViens.ToList().OrderBy(n => n.MaNV).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ThemNhanVien()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemNhanVien(NhanVien item)
        {
            db.NhanViens.InsertOnSubmit(item);
            db.SubmitChanges();
            return RedirectToAction("NhanVien");
        }
        public ActionResult XoaNV(int id)
        {
            NhanVien item = db.NhanViens.SingleOrDefault(n => n.MaNV == id);
            ViewBag.MaNV = item.MaNV;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        [HttpPost, ActionName("XoaNV")]
        public ActionResult XacNhanXoaNV(int id)
        {
            NhanVien item = db.NhanViens.SingleOrDefault(n => n.MaNV == id);
            ViewBag.MaNV = item.MaNV;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.NhanViens.DeleteOnSubmit(item);
            db.SubmitChanges();
            return RedirectToAction("NhanVien");
        }
        public ActionResult ChiTietNV(int id)
        {
            NhanVien item = db.NhanViens.SingleOrDefault(n => n.MaNV == id);
            ViewBag.MaNV = item.MaNV;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        public ActionResult SuaNV(int id)
        {
            NhanVien item = db.NhanViens.SingleOrDefault(n => n.MaNV == id);
            return View(item);
        }
        [HttpPost]
        public ActionResult SuaNV(NhanVien nv)
        {
            NhanVien itemm = db.NhanViens.SingleOrDefault(n => n.MaNV == nv.MaNV);
            itemm.TenNV = nv.TenNV;
            itemm.GioiTinh = nv.GioiTinh;
            itemm.NgaySinh = nv.NgaySinh;
            itemm.DiaChi = nv.DiaChi;
            itemm.SDT = nv.SDT;
            itemm.Email = nv.Email;
            itemm.ChucVu = nv.ChucVu;
            itemm.Luong = nv.Luong;
            db.SubmitChanges();
            return RedirectToAction("NhanVien");
        }
        public ActionResult DonDatHang(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.DonDatHangs.ToList().OrderBy(n => n.MaDonHang).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult SuaDDH(int id)
        {
            DonDatHang item = db.DonDatHangs.SingleOrDefault(n => n.MaDonHang == id);
            return View(item);
        }
        [HttpPost]
        public ActionResult SuaDDH(DonDatHang ddh)
        {
            DonDatHang itemm = db.DonDatHangs.SingleOrDefault(n => n.MaDonHang == ddh.MaDonHang);
            if (itemm.TinhTrangGiaohang == null)
            {
                itemm.TinhTrangGiaohang = ddh.TinhTrangGiaohang;
                db.SubmitChanges();
            }
            else
            itemm.TinhTrangGiaohang = ddh.TinhTrangGiaohang;
            if (itemm.DaThanhToan == null)
            {
                itemm.DaThanhToan = ddh.DaThanhToan;
                db.SubmitChanges();
            }    
            else
            itemm.DaThanhToan = ddh.DaThanhToan;
            db.SubmitChanges();
            return RedirectToAction("DonDatHang");
        }
        public ActionResult ChiTietDH(int id)
        {
            ChiTietDatHang item = db.ChiTietDatHangs.FirstOrDefault(n => n.MaDonHang == id);
            ViewBag.MaDonHang = item.MaDonHang;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        public ActionResult XoaDDH(int id)
        {
            DonDatHang item = db.DonDatHangs.SingleOrDefault(n => n.MaDonHang == id);
            ViewBag.MaDonHang = item.MaDonHang;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        [HttpPost, ActionName("XoaDDH")]
        public ActionResult XacNhanXoaDDH(int id)
        {
            DonDatHang item = db.DonDatHangs.SingleOrDefault(n => n.MaDonHang == id);
            ViewBag.MaDonHang = item.MaDonHang;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.DonDatHangs.DeleteOnSubmit(item);
            db.SubmitChanges();
            return RedirectToAction("DonDatHang");
        }
    }
}

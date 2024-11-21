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
using Newtonsoft.Json;

namespace NDKFastfood.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        dbKiwiFastfoodDataContext db = new dbKiwiFastfoodDataContext(ConfigurationManager.ConnectionStrings["KiwiFastfoodConnectionString1"].ConnectionString);

        public ActionResult Index()
        {
            return RedirectToAction("Login", "Admin");
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

        //Mon an
        public ActionResult MonAn(int? page)
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
            ViewBag.MaLoai = new SelectList(db.Loais.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh đại diện";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var filename = Path.GetFileName(fileupload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Assets/Images/"), filename);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileupload.SaveAs(path);
                    }
                    monan.AnhDD = filename;
                    db.MonAns.InsertOnSubmit(monan);
                    db.SubmitChanges();
                }
                return RedirectToAction("MonAn");
            }
        }

        //Chi tiet mon an
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

        //Loai
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

        //Khach hang
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

        //Nhan vien
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

        //Don hang
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

        //Khuyen mai
        public ActionResult KhuyenMai(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.KhuyenMais.ToList().OrderBy(n => n.MaKM).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ThemKhuyenMai()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemKhuyenMai(KhuyenMai item)
        {
            db.KhuyenMais.InsertOnSubmit(item);
            db.SubmitChanges();
            return RedirectToAction("KhuyenMai");
        }
        public ActionResult XoaKM(int id)
        {
            KhuyenMai item = db.KhuyenMais.SingleOrDefault(n => n.MaKM == id);
            ViewBag.MaKM = item.MaKM;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        [HttpPost, ActionName("XoaKM")]
        public ActionResult XacNhanXoaKM(int id)
        {
            KhuyenMai item = db.KhuyenMais.SingleOrDefault(n => n.MaKM == id);
            ViewBag.MaKM = item.MaKM;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.KhuyenMais.DeleteOnSubmit(item);
            db.SubmitChanges();
            return RedirectToAction("KhuyenMai");
        }
        public ActionResult ChiTietKM(int id)
        {
            KhuyenMai item = db.KhuyenMais.SingleOrDefault(n => n.MaKM == id);
            ViewBag.MaKM = item.MaKM;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        public ActionResult SuaKM(int id)
        {
            KhuyenMai item = db.KhuyenMais.SingleOrDefault(n => n.MaKM == id);
            return View(item);
        }
        [HttpPost]
        public ActionResult SuaKM(KhuyenMai km)
        {
            KhuyenMai item = db.KhuyenMais.SingleOrDefault(n => n.MaKM == km.MaKM);
            item.TenKM = km.TenKM;
            item.PhanTramGiamGia = km.PhanTramGiamGia;
            item.NgayBatDau = km.NgayBatDau;
            item.NgayKetThuc = km.NgayKetThuc;
            item.SoLuong = km.SoLuong;
            db.SubmitChanges();
            return RedirectToAction("KhuyenMai");
        }

        //Danh gia
        public ActionResult DanhGia(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.DanhGias.ToList().OrderBy(n => n.MaDG).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ThemDanhGia()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemDanhGia(DanhGia item)
        {
            db.DanhGias.InsertOnSubmit(item);
            db.SubmitChanges();
            return RedirectToAction("DanhGia");
        }
        public ActionResult XoaDG(int id)
        {
            DanhGia item = db.DanhGias.SingleOrDefault(n => n.MaDG == id);
            ViewBag.MaDG = item.MaDG;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        [HttpPost, ActionName("XoaDG")]
        public ActionResult XacNhanXoaDG(int id)
        {
            DanhGia item = db.DanhGias.SingleOrDefault(n => n.MaDG == id);
            ViewBag.MaDG = item.MaDG;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.DanhGias.DeleteOnSubmit(item);
            db.SubmitChanges();
            return RedirectToAction("DanhGia");
        }
        public ActionResult ChiTietDG(int id)
        {
            DanhGia item = db.DanhGias.SingleOrDefault(n => n.MaDG == id);
            ViewBag.MaDG = item.MaDG;
            if (item == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(item);
        }
        public ActionResult SuaDG(int id)
        {
            DanhGia item = db.DanhGias.SingleOrDefault(n => n.MaDG == id);
            return View(item);
        }
        [HttpPost]
        public ActionResult SuaDG(DanhGia dg)
        {
            DanhGia itemm = db.DanhGias.SingleOrDefault(n => n.MaDG == dg.MaDG);
            itemm.NoiDung = dg.NoiDung;
            db.SubmitChanges();
            return RedirectToAction("DanhGia");
        }

        //Thong ke doanh thu
        public ActionResult ThongKeDoanhThu(int? month, int? year)
        {
            month = month ?? DateTime.Now.Month;
            year = year ?? DateTime.Now.Year;

            var doanhThu = db.DonDatHangs
                .Where(ddh => ddh.NgayDat.HasValue && ddh.NgayDat.Value.Month == month && ddh.NgayDat.Value.Year == year && ddh.DaThanhToan == false)
                .Join(db.ChiTietDatHangs,
                      ddh => ddh.MaDonHang,
                      ctdh => ctdh.MaDonHang,
                      (ddh, ctdh) => new { ddh, ctdh })
                .GroupBy(x => new { x.ddh.NgayDat.Value.Month, x.ddh.NgayDat.Value.Year, x.ddh.NgayDat.Value.Day })
                .Select(g => new
                {
                    Ngay = g.Key.Day,
                    TongDoanhThu = g.Sum(x => x.ctdh.SoLuong * x.ctdh.DonGia)
                })
                .OrderBy(x => x.Ngay)
                .ToList();

            if (doanhThu.Any())
            {
                ViewBag.DanhSachDoanhThu = JsonConvert.SerializeObject(doanhThu);
                ViewBag.Thang = month;
                ViewBag.Nam = year;
            }
            else
            {
                ViewBag.DanhSachDoanhThu = "[]"; 
                ViewBag.Thang = month;
                ViewBag.Nam = year;
            }

            return View();
        }
    }
}

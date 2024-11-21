using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NDKFastfood.Models;
namespace NDKFastfood.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        dbKiwiFastfoodDataContext data = new dbKiwiFastfoodDataContext(ConfigurationManager.ConnectionStrings["KiwiFastfoodConnectionString1"].ConnectionString);
        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
        public ActionResult ThemGioHang(int @iMaMon, string strUrl)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang monan = lstGioHang.Find(n => n.iMaMon == @iMaMon);
            if (monan == null)
            {
                monan = new GioHang(@iMaMon);
                lstGioHang.Add(monan);
            }
            else
            {
                monan.iSoLuong++;
            }

            if (Session["MaGiamGia"] != null)
            {
                Session["MaGiamGia"] = null;
            }

            return Redirect(strUrl);
        }

        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }

        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;

            if (lstGioHang != null)
            {
                iTongTien = lstGioHang.Sum(n => n.dThanhTien);

                if (Session["MaGiamGia"] != null)
                {
                    KhuyenMai magg = (KhuyenMai)Session["MaGiamGia"];
                    if (magg != null)
                    {
                        double discount = (double)magg.PhanTramGiamGia.GetValueOrDefault();
                        iTongTien -= iTongTien * (discount / 100);
                    }
                }
            }
            return iTongTien;
        }

        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }

        public ActionResult GioHangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }

        public ActionResult XoaGiohang(int iMaMon)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang monan = lstGioHang.SingleOrDefault(n => n.iMaMon == iMaMon);
            if (monan != null)
            {
                lstGioHang.RemoveAll(n => n.iMaMon == iMaMon);
                return RedirectToAction("GioHang");
            }
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult CapNhatGioHang(int iMaMon, FormCollection f)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang monan = lstGioHang.SingleOrDefault(n => n.iMaMon == iMaMon);
            if (monan != null)
            {
                monan.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult XoaTatCaGioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            Session["MaGiamGia"] = null;
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            // lấy giỏ hàng từ session
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }

        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            List<GioHang> gioHang = LayGioHang();  // Giả sử đây là giỏ hàng của bạn

            DonDatHang donDatHang = new DonDatHang
            {
                MaKH = kh.MaKH,
                NgayDat = DateTime.Now,
                NgayGiao = DateTime.Parse(collection["Ngaygiao"]),
                TinhTrangGiaohang = false,
                DaThanhToan = false
            };

            data.DonDatHangs.InsertOnSubmit(donDatHang);
            data.SubmitChanges();

            double tongTien = 0; // Biến để lưu tổng tiền sau khi tính giảm giá

            foreach (var item in gioHang)
            {
                double giaSauGiam = (double)item.dGiaBan;  // Giá gốc

                // Kiểm tra xem có mã giảm giá không
                if (Session["MaGiamGia"] != null)
                {
                    KhuyenMai magiamgia = (KhuyenMai)Session["MaGiamGia"];
                    // Tính giá sau giảm cho mỗi món
                    giaSauGiam = giaSauGiam - giaSauGiam * ((double)magiamgia.PhanTramGiamGia.GetValueOrDefault() / 100);
                }

                // Tính tổng tiền cho món này
                double thanhTienMon = giaSauGiam * item.iSoLuong;

                // Lưu chi tiết đơn hàng với giá sau giảm
                ChiTietDatHang chiTiet = new ChiTietDatHang
                {
                    MaDonHang = donDatHang.MaDonHang,
                    MaMon = item.iMaMon,
                    SoLuong = item.iSoLuong,
                    DonGia = (decimal)giaSauGiam  // Lưu giá sau giảm vào DonGia
                };

                data.ChiTietDatHangs.InsertOnSubmit(chiTiet);

                // Cộng tổng tiền cho đơn hàng
                tongTien += thanhTienMon;
            }

            data.SubmitChanges();

            // Kiểm tra và giảm số lượng mã giảm giá sau khi sử dụng
            if (Session["MaGiamGia"] != null)
            {
                KhuyenMai magiamgia = data.KhuyenMais.FirstOrDefault(k => k.MaKM == ((KhuyenMai)Session["MaGiamGia"]).MaKM);

                if (magiamgia != null && magiamgia.SoLuong > 0)
                {
                    magiamgia.SoLuong--;  // Giảm số lượng mã giảm giá
                    data.SubmitChanges();
                }
                else
                {
                    TempData["ErrorMessage"] = "Mã giảm giá không còn lượt sử dụng!";
                }
            }

            // Xóa giỏ hàng và mã giảm giá khỏi session
            Session["GioHang"] = null;
            Session["MaGiamGia"] = null;

            // Trả về tổng tiền (nếu bạn cần hiển thị trong trang xác nhận đơn hàng)
            ViewBag.TongTien = tongTien;

            return RedirectToAction("XacNhanDonHang", "GioHang");
        }

        public ActionResult XacNhanDonHang()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ApDungMaGiamGia(string txtMaGiamGia)
        {
            KhuyenMai magg = data.KhuyenMais.SingleOrDefault(m => m.MaKM.ToString() == txtMaGiamGia);

            if (magg == null)
            {
                TempData["ErrorMessage"] = "Mã giảm giá không hợp lệ!";
                return RedirectToAction("GioHang");
            }

            if (magg.NgayKetThuc < DateTime.Now)
            {
                TempData["ErrorMessage"] = "Mã giảm giá đã hết hạn!";
                return RedirectToAction("GioHang");
            }

            if (magg.SoLuong.HasValue && magg.SoLuong <= 0)
            {
                TempData["ErrorMessage"] = "Mã giảm giá đã hết lượt sử dụng!";
                return RedirectToAction("GioHang");
            }

            if (Session["MaGiamGia"] != null)
            {
                TempData["ErrorMessage"] = "Mã giảm giá đã được áp dụng!";
                return RedirectToAction("GioHang");
            }

            Session["MaGiamGia"] = magg;

            List<GioHang> lstGioHang = LayGioHang();
            double tongTien = TongTien();
            double tongTienSauGiam = tongTien - (tongTien * (double)magg.PhanTramGiamGia.GetValueOrDefault() / 100);

            ViewBag.TongTien = tongTienSauGiam;

            TempData["SuccessMessage"] = $"Mã giảm giá '{magg.TenKM}' đã được áp dụng! Bạn được giảm {magg.PhanTramGiamGia.GetValueOrDefault()}%.";

            return RedirectToAction("GioHang");
        }
    }
}
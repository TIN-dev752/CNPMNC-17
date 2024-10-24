using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace NDKFastfood.Models
{
    public class GioHang
    {
        private dbKiwiFastfoodDataContext data;

        public GioHang()
        {
            // Lấy chuỗi kết nối từ Web.config
            string connectionString = ConfigurationManager.ConnectionStrings["KiwiFastfoodConnectionString"].ConnectionString;

            // Khởi tạo đối tượng dbKiwiFastfoodDataContext với chuỗi kết nối
            data = new dbKiwiFastfoodDataContext(connectionString);
        }
        public int iMaMon { set; get; }
        public string sTenMon { set; get; }
        public double dGiaBan { set; get; }
        public string sAnhDD { set; get; }
        public int iSoLuong { set; get; }
        public double dThanhTien
        {
            get { return iSoLuong * dGiaBan; }
        }
        public GioHang(int MaMon)
        {
            iMaMon = MaMon;
            MonAn monan = data.MonAns.Single(n => n.MaMon == iMaMon);
            sTenMon = monan.TenMon;
            sAnhDD = monan.AnhDD;
            dGiaBan = double.Parse(monan.GiaBan.ToString());
            iSoLuong = 1;
        }
    }
}
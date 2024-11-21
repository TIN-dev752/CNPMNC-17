using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NDKFastfood.Models
{
    public class TDanhGia
    {
        public int MaDG { get; set; } 
        public int MaMon { get; set; } 
        public int MaKH { get; set; } 
        public string NoiDung { get; set; } 
        public DateTime NgayDanhGia { get; set; } 
    }
}
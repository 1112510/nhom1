using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuanLiThuVien.Models;

namespace QuanLiThuVien.Controllers
{
    public class database
    {
        private QLThuVienEntities5 db;
        public database()
        {
            db= new QLThuVienEntities5();
        }
        public QLThuVienEntities5 d() {
            return db;
        }
        public void Renew()
        {
            db = new QLThuVienEntities5();
        }
    }
}
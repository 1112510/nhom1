using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLiThuVien.Models;
using Limilabs.Barcode;
using System.Web.Helpers;
using System.Drawing;
using System.Web.UI.DataVisualization;
using System.Web.UI.DataVisualization.Charting;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace QuanLiThuVien.Controllers
{
    public class QLDGController : Controller
    {
        public static database db;
        public ActionResult main()
        {
            //ql = new QLThuVienEntities5();
            db = new database();
            return View();
        }
        [HttpPost]
        public PartialViewResult PhatSinh()
        {
            //QLThuVienEntities1 ql = new QLThuVienEntities1();
            List<tbl_DocGia_Tam> lst = (from e in db.d().tbl_DocGia_Tam select e).ToList();
            return PartialView("PhatSinh", lst);
        }
        public bool MoveImage(string dg, string newDG) {
            bool kq = true;
            try
            {
                string path1 = Server.MapPath("~/Content/images/docGia_Tam");
                string[] Files = Directory.GetFiles(path1, dg.Trim() + ".png")
                                         .Select(path => Path.GetFileName(path))
                                         .ToArray();
                
                string path2 = Server.MapPath("~/Content/images/docgia");
                Image t = Image.FromFile(path1 + "/" + Files[0]); //.Save(path2 + "/" + Files[0].Split('.')[0].ToLower() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                t.Save(path2 + "/" + newDG + ".png", System.Drawing.Imaging.ImageFormat.Png);
                t.Dispose();
                FileInfo file = new FileInfo(path1 + "/" + Files[0]);
                file.Delete();
            }
            catch (Exception ex)
            {
                kq = false;
            }
            return kq;
        }
        public tbl_DocGia GanDocGia(string masothe, tbl_DocGia_Tam dg)  //gan doc gia tam (tbl_DocGiaTam) vao bang doc gia chinh (tbl_DocGia)
        {
            tbl_DocGia docgiamoi = new tbl_DocGia();
            docgiamoi.ID = masothe;
            docgiamoi.Ten = dg.Ten;
            docgiamoi.DiaChi = dg.DiaChi;
            docgiamoi.Email = dg.Email;
            docgiamoi.SDT = dg.SDT;
            docgiamoi.CMND = dg.CMND;
            docgiamoi.MSSV = dg.MSSV;
            docgiamoi.Khoa = dg.Khoa;
            docgiamoi.NgaySinh = dg.NgaySinh;
            docgiamoi.GioiTinh = dg.GioiTinh;
            docgiamoi.NgayCapNhat = dg.NgayCapNhat;
            docgiamoi.NgayHetHan = dg.NgayHetHan;
            docgiamoi.User = "n";
            docgiamoi.Pass = dg.Pass ;
            return docgiamoi;
        }
        public string PhatSinhTenHinhAnh()         //phat sinh ten hinh anh cho anh doc gia trong bang chinh
        {
            string masothe = "";
            var stt = (from e in db.d().tbl_DocGia select e.CMND);
            string head = stt.Count().ToString();
            int soluongkitu = stt.Count().ToString().Length;
            for (int i = 0; i < (4 - soluongkitu); i++)
            {
                head = "0" + head;
            }

            string end = DateTime.Now.ToString().Trim();
            end = end.Substring(0, 4);
            //dinh nghia ma so the
            masothe = end + head;
            return masothe;
        }
        [HttpPost]
        public PartialViewResult Barcode_MaSoThe()  //t va t.id phai cung ten voi Jquery trong PhatSinh.cshtml
        { 
            int id =int.Parse( RouteData.Values["id"].ToString());
            tbl_DocGia_Tam dg = (from e in db.d().tbl_DocGia_Tam
                                where e.ID == id
                                select
                                e
                                ).First();
            string masothe = PhatSinhTenHinhAnh();                //phat sinh ten hinh anh / ma cua doc gia moi
            tbl_DocGia docgiamoi = GanDocGia(masothe, dg);
            db.d().tbl_DocGia.Add(docgiamoi);
            db.d().SaveChanges();
           
            //*************gui mail cho doc gia xac nhan la da duoc them moi thanh cong
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("lamtrongnhan0505@gmail.com", "codonmai"),
                EnableSsl = true
            };
            string title = "Thu vien Dai Hoc Khoa Hoc Tu Nhien";
            string body = "chao " + docgiamoi.Ten + "; \n Ban da duoc tro thanh thanh vien cua thu vien: \n Username: " + docgiamoi.User + "\n Password: " + docgiamoi.Pass + "\n Xin cam on";
            client.Send("lamtrongnhan0505@gmail.com", docgiamoi.Email.ToString(), title, body);
            //************

            //*********** phat sinh barcode cho doc gia
            BaseBarcode barcode = BarcodeFactory.GetBarcode(Symbology.Code11);
            barcode.Number = masothe;
            barcode.ChecksumAdd = true;
            string path = Server.MapPath(@"~\Content\images\barcode\" + masothe + ".gif");   //luu barcode vua tao
            barcode.Save(path, ImageType.Gif);
            //************

            if (!MoveImage(dg.CMND.ToString(), masothe))
            {
                return PartialView("Error","home");
            }

            //*************xoa doc gia vua tao trong bang tbl_DocGiaTam
            db.d().tbl_DocGia_Tam.Remove(dg);
            db.d().SaveChanges();
            db.Renew();
            //************
            
            //************ load lai trang web
            return PhatSinh();  
        }
        
        public PartialViewResult TK_DG_Khoa()
        {
            //ql = new QLThuVienEntities5();
            var data = (from dg in db.d().tbl_DocGia
                       group dg by dg.Khoa into m
                       select new { tong = m.Count(), ten = m.Key });
            List<DocGia_TK> dg1 = new List<DocGia_TK>();
            System.Web.UI.DataVisualization.Charting.Chart Chart2 = new System.Web.UI.DataVisualization.Charting.Chart();
           
            Chart2.Width = 500;
            Chart2.Height = 400;
            Chart2.ChartAreas.Add("thong ke khoa").BackColor = System.Drawing.Color.FromArgb(64, System.Drawing.Color.White);

            Chart2.Palette = ChartColorPalette.EarthTones;
            Chart2.Titles.Add("THỐNG KÊ THEO KHOA");
            Series series = Chart2.Series.Add("thong ke");
            Chart2.Series.Clear();
            Chart2.Series.Add(series);
            Chart2.Series[0].ChartType = SeriesChartType.Pie;
            
            int tongSo = 0;
            foreach (var tam in data)
            {
                if (tam.ten != null)
                {
                    DocGia_TK tt = new DocGia_TK();
                    tt.ten = tam.ten;
                    tt.tong = tam.tong;
                    dg1.Add(tt);
                    tongSo += tam.tong;
                    series.Points.AddXY(tam.ten, tam.tong);
                }
            }
            ViewBag.tongSo = tongSo.ToString();
            Chart2.BackColor = Color.Transparent;
            //MemoryStream imageStream = new MemoryStream();
            string nam = DateTime.Now.ToString().Trim();
            nam = nam.Substring(0, 4)+nam.Substring(4,3);
            string path = Server.MapPath(@"~\Content\images\ThongKeKhoa" + nam+".png" );
            Chart2.SaveImage(path, ChartImageFormat.Png);
            //Chart2.TextAntiAliasingQuality = TextAntiAliasingQuality.SystemDefault;
            //Response.ContentType = "image/png";
            ViewBag.link = @"../Content/images/ThongKeKhoa" + nam + ".png";  //path.Replace('\\', '/');         
            return PartialView("TK_DG_Khoa", dg1);
        }
        public PartialViewResult TK_DG_Nam()
        {    
            List<DocGia_TK> dd = new List<DocGia_TK>();
            var data = from dg in db.d().tbl_DocGia
                       group dg by dg.NgayCapNhat.Value.Year into m
                       select new { tong = m.Count(), ten = m.Key };
            System.Web.UI.DataVisualization.Charting.Chart Chart2 = new System.Web.UI.DataVisualization.Charting.Chart();

            Chart2.Width = 500;
            Chart2.Height = 400;
            Chart2.ChartAreas.Add("thong ke nam").BackColor = System.Drawing.Color.FromArgb(64, System.Drawing.Color.White);

            Chart2.Palette = ChartColorPalette.Chocolate;
            Chart2.Titles.Add("THÔNG KÊ THEO NĂM");
            Series series = Chart2.Series.Add("thong ke");
            Chart2.Series.Clear();
            Chart2.Series.Add(series);
            Chart2.Series[0].ChartType = SeriesChartType.Line;
            int tongSo = 0;
            foreach (var tam in data)
            {
                tongSo += tam.tong;
                DocGia_TK h = new DocGia_TK();
                h.ten = tam.ten.ToString();
                h.tong = tam.tong;
                dd.Add(h);
                series.Points.AddXY(tam.ten, tam.tong);
            }
            ViewBag.tongSo = tongSo.ToString();
            Chart2.BackColor = Color.White;
            //MemoryStream imageStream = new MemoryStream();
            string nam = DateTime.Now.ToString().Trim();
            nam = nam.Substring(0, 4) + nam.Substring(4, 3);
            string path = Server.MapPath(@"~\Content\images\ThongKeNam" + nam + ".png");
            Chart2.SaveImage(path, ChartImageFormat.Png);
            //Chart2.TextAntiAliasingQuality = TextAntiAliasingQuality.SystemDefault;
            //Response.ContentType = "image/png";
            ViewBag.linkNam = @"../Content/images/ThongKeNam" + nam + ".png";  //path.Replace('\\', '/');         
            return PartialView("TK_DG_Nam", dd);
        }
        public PartialViewResult DS_DocGia()
        {
            List<tbl_DocGia> t = (from e in db.d().tbl_DocGia select e ).ToList();
            return PartialView("View_DSDocGia",t);
        }
        /*public ActionResult Mail()
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("lamtrongnhan0505@gmail.com", "tieu diet"),
                EnableSsl = true
            };
            client.Send("lamtrongnhan0505@gmail.com", "lamtrongnhan0505@gmail.com", "test", "testbody");
            return null;
        }*/
        [HttpGet]
        public ActionResult InThe (string id)
        {     
            tbl_DocGia t = (from e in db.d().tbl_DocGia where e.ID==id select e).First();
            return View(t);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuanLiThuVien.Models;
using System.Web.Mvc;

namespace QuanLiThuVien.Controllers
{
    public class GuestController : Controller
    {
        //
        // GET: /Guest/
        public static database db;
        public ActionResult Login()
        {
            db = new database();
            return View();
        }

        [HttpGet]
        public ActionResult Login1()
        {
            string username = Request["user"].ToString();
            string password = Request["pass"].ToString();
            int loai = -1;
            loai=IsValid(username, password);
            switch(loai){
                case 1:
                {           //dua ve giao dien nhan vien quan ly doc gia
                    Session["username"] = username;
                    Session["loai"] = loai;
                    return RedirectToAction("main","QLDG"); 
                }
                case 0: 
                { 
                    Session["username"] = username;
                    return RedirectToAction("main", "HoTroDocGia");
                }
                default: return RedirectToAction("Login");
            }
        }

        private int IsValid(string username, string password)
        {
            int IsValid = -1;
            
            var user = (from e in db.d().tbl_NhanVien where e.ID == username & e.Password == password select e).ToList();
            if (user.Count!=0)
            {
                foreach (var n in user)
                {
                    IsValid = n.LoaiNV;
                }
            }
            else
            {
                var doc_gia = (from e in db.d().tbl_DocGia where e.ID == username & e.Pass == password select e).ToList();
                if (doc_gia.Count != 0)
                {
                    foreach (var n in doc_gia)
                    {
                        IsValid = 0;
                    }
                }
            }
            return IsValid;
            
        }  
      
        public ActionResult HocOnline()
        {
            //QLThuVienEntities5 ql = new QLThuVienEntities5();    
            db = new database();
            var ds = (from e in db.d().Answers select e).ToList();
            return View(ds);
        }

        [HttpPost]
        public ActionResult TracNghiem()
        {
            //QLThuVienEntities5 ql = new QLThuVienEntities5();
            var list_ans = (from s in db.d().Questions select s).ToList();
            int mnc = 0;

            if (ModelState.IsValid)
            {
                foreach (var item in list_ans)
                {

                    string id = item.ID_q.ToString();
                    if (Request[id] == null)
                    {
                        continue;
                    }
                    int bar = int.Parse(Request[id].ToString());

                    var ketqua = (from kq in db.d().Answers
                                  where kq.ID_a == bar && kq.Answer_choice == true
                                  group kq by kq.ID_q into abc
                                  select new { t= abc.Key ,tong = abc.Count()});

                    foreach (var abcd in ketqua)
                    {
                        mnc += abcd.tong;
                    }
                }
            }
            if (mnc >= 3)
            {
                return RedirectToAction("Register");
            }
            TempData["thatbai"] = "Chưa đủ số câu trả lời đúng";
            return RedirectToAction("HocOnline");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register1(HttpPostedFileBase file)
        {
            //using (QLThuVienEntities5 db = new QLThuVienEntities5())
            //{
            // nhung thong tin nhu máv, khoa, ngay het han dc de null
            //email = Request["txtemail"];
            // tao doc gia

            tbl_DocGia_Tam doc_gia = new tbl_DocGia_Tam();
            //doc_gia.ID = 3;
            doc_gia.Ten = Request["txtho_ten"];
            doc_gia.DiaChi = Request["txtdia_chi"];
            doc_gia.SDT = null;
            doc_gia.Email = Request["txt_email"];
            // can than thuoc tinh ngay sinh, nhap phai dung dang
            // de datetime ngay sinh trong csdl
            doc_gia.NgaySinh = Convert.ToDateTime(Request["txt_date"]);
            doc_gia.CMND = Request["txt_cmnd"];
            doc_gia.MSSV = null;
            doc_gia.Khoa = null;
            doc_gia.GioiTinh = Request["txtgioi_tinh"];
            // thoi gian lap tai khoan
            doc_gia.NgayCapNhat = DateTime.Now;
            doc_gia.NgayHetHan = null;
            doc_gia.Pass = Request["txt_pass"];
            //user.Email = duongdan("1111", file);
            // kiem tra file
            if (file != null)
            {
                string filename = System.IO.Path.GetFileName(file.FileName);
                // luu anh vao thu muc vooi ten la cmnd

                file.SaveAs(Server.MapPath("~/Content/images/docgia_tam/" + Request["txt_cmnd"] + ".png"));

                //doc_gia.AnhUser = true;

            }
            else
            {
                // truong hop khong co anh tra no ve lai trang register
                return RedirectToAction("Register");
            }
            // save 
            db.d().tbl_DocGia_Tam.Add(doc_gia);
            db.d().SaveChanges();
            db.Renew();
            return RedirectToAction("Index","home");
        }

    }
}

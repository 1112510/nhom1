//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuanLiThuVien.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_DocGia
    {
        public tbl_DocGia()
        {
            this.phieumuons = new HashSet<phieumuon>();
        }
    
        public string ID { get; set; }
        public string Ten { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public string SDT { get; set; }
        public string CMND { get; set; }
        public string MSSV { get; set; }
        public string Khoa { get; set; }
        public Nullable<System.DateTime> NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public Nullable<System.DateTime> NgayCapNhat { get; set; }
        public Nullable<System.DateTime> NgayHetHan { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
    
        public virtual ICollection<phieumuon> phieumuons { get; set; }
    }
}
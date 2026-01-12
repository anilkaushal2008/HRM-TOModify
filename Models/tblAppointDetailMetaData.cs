using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class tblAppointDetailMetaData
    {
        [Display(Name ="Appointment master")]
        [Required(ErrorMessage ="Select appointment master")]
        public int fk_AppointMasid { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Enter name")]
        public string vchName { get; set; }

        [Display(Name = "Father name")]
        [Required(ErrorMessage = "Enter father name")]
        public string vchFatherName { get; set; }

        [Display(Name = "Application date")]
        [Required(ErrorMessage = "Select application date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime dtApplication { get; set; }

        [Display(Name = "DOJ")]
        [Required(ErrorMessage = "Select DOJ")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime dtDOJ { get; set; }

        [Display(Name = "Content")]
        [Required(ErrorMessage = "Enter content")]
        public string txtContent { get; set; }

        [Display(Name = "Creation date")]        
        public System.DateTime dtCreated { get; set; }

        [Display(Name = "HRMS ID")]
        [Required(ErrorMessage = "Select hrms id")]
        public Nullable<int> fk_hrmsEMPid { get; set; }

        [Display(Name = "State")]
        public string vchState { get; set; }

        [Display(Name = "City")]
        public string vchCity { get; set; }

        [Display(Name = "Address")]
        public string vchAddress { get; set; }

        [Display(Name = "Department")]
        public int fk_department { get; set; }

        [Display(Name = "Designation")]
        public int fk_designation { get; set; }

        [Display(Name ="CTC")]
        [Required(ErrorMessage ="Enter CTC")]
        public int intCTC { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Select gender")]
        public string vchGender { get; set; }



    }
}
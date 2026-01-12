using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class tblIntershipDetailMetaData
    {
        [Display(Name ="Select master")]
        [Required(ErrorMessage ="Select master")]
        public int fk_Masid { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Enter name")]
        public string vchName { get; set; }

        [Display(Name = "Father name")]
        [Required(ErrorMessage = "Enter father name")]
        public string vchFatherName { get; set; }

        [Display(Name = "Application Date")]
        [Required(ErrorMessage = "Select application date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime dtApplication { get; set; }

        [Display(Name = "Date from")]
        [Required(ErrorMessage = "Select master date from")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime dtDOS { get; set; }

        [Display(Name = "Date to")]
        [Required(ErrorMessage = "Select date to")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime dtDOE { get; set; }

        [Display(Name = "Content")]
        [Required(ErrorMessage = "Enter content")]
        public string txtContent { get; set; }
        
        [Required(ErrorMessage = "Select is hrms employee or not")]
        public bool bitIsHRMSemp { get; set; }
       

        [Display(Name = "State")]
        [Required(ErrorMessage = "Enter State")]        
        public string vchState { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "Enter city")]
        public string vchCity { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Enter address")]
        public string vchAddress { get; set; }

        [Display(Name = "Department")]
        [Required(ErrorMessage = "Select department")]
        public int fk_department { get; set; }

        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Select designation")]
        public int fk_designation { get; set; }

        [Display(Name="Gender")]
        [Required(ErrorMessage ="Select gender")]
        public string vchGender { get; set; }
    }
}
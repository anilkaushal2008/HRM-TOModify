using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class tblExpDetailMetaData
    {
        [Display(Name="Select master type")]
        [Required(ErrorMessage = "Select master type")]
        public int fk_Masid { get; set; }

        [Display(Name ="Employee name")]
        [Required(ErrorMessage = "Enter employee name")]
        public string vchName { get; set; }

        [Display(Name ="Father name")]
        [Required(ErrorMessage ="Enter father name")]
        public string vchFatherName { get; set; }

        [Display(Name = "Department")]
        [Required(ErrorMessage = "Select department")]
        public int fk_department { get; set; }

        [Display(Name ="Designation")]
        [Required(ErrorMessage ="Select designation")]
        public int fk_designationId { get; set; }

        [Display (Name ="Content")]
        [Required(ErrorMessage ="Enter letter content")]
        public string txtContent { get; set; }

        [Display(Name ="Date from")]
        [Required(ErrorMessage ="select date start from")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime dtSdate { get; set; }

        [Display(Name = "Date to")]
        [Required(ErrorMessage = "select to date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime? dtEdate { get; set; }

        [Display(Name = "Select relieving date")]
        [Required(ErrorMessage = "Select relieving date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime? dtRelieving { get; set; }
        
        [Display(Name = "Creation date")]        
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime dtCreated { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Enter State")]
        public string vchState { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "Enter city")]
        public string vchCity { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Enter address")]
        public string vchAddress { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Select gender")]
        public string vchGender { get; set; }
    }
}
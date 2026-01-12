using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class ConsultRegistration
    {
        public int empID { get; set; }
        [Display(Name ="Mobile")]
        public string mobile { get; set; }
    
        [Display(Name ="Name")]
        public string Name { get; set; }

        [Display(Name="Temp. Code")]
        public string EmpTcode { get; set; }

        [Display(Name="Registration Date from")]
        [Required(ErrorMessage ="Select date from")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:d}"))]
        public DateTime dtRegiFrom { get; set; }

        [Display(Name = "Registration Date ")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:d}"))]
        public DateTime dtRegistration { get; set; }

        [Display(Name = "Registration date to")]
        [Required(ErrorMessage = "Select to date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:d}"))]
        public DateTime dtRegito { get; set; }
        
        [Display(Name = "Enter file name")]
        [Required(ErrorMessage = "Enter file name")]
        public string fileName { set; get; }

        [Display(Name = "Upload files")]
        [Required(ErrorMessage = "Select pdf file")]      
        public HttpPostedFileBase pdfFile { set; get; }

       

    }
}
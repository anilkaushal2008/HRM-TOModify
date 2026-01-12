using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc; 

namespace HRM.Models
{
    public class UpPartialAuthDocViewModel
    {
        public int empid { get; set; }

        public int fk_docid { get; set; }

        [Required(ErrorMessage = "Select pdf file")]
        [Display(Name = "Upload files")]      
        public HttpPostedFileBase newpdfFile { set; get; }

        [Display(Name = "Upload PDF")]
        public string newfilename { set; get; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class UpConsultantDocModel
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class UploadLedgerViewModel
    {
        public int empid { get; set; }

        [Display(Name = "Document name")]
        public string DocMasName { get; set; }

        [Required(ErrorMessage = "Select pdf file")]
        [Display(Name = "Upload files")]
        public HttpPostedFileBase newpdfFile { set; get; }

        [Display(Name = "Upload PDF")]
        public string Filename { set; get; }       
       
        public string intYear { get; set; }
    }
}

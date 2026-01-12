using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc; 

namespace HRM.Models
{
    public class DocumentViewModel
    {
        [Required (ErrorMessage ="Select file")]
        [Display(Name ="Upload files")]
        // public HttpPostedFileBase[] pdfFile { set; get; }
        public HttpPostedFileBase pdfFile { set; get; }
        
        [Display(Name ="Upload PDF")]
        [Required(ErrorMessage ="Please select document name")]
        public string filename { set; get; }
        public IEnumerable<SelectListItem> docnamelist { set; get; }

        public int empid { set; get; }
        public Boolean BitCompleted { set; get; }
    }
}
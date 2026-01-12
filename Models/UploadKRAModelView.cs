using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace HRM.Models
{
    public class UploadKRAModelView
    {
        public int empid { get; set; }       

        [Display(Name = "Document name")]
        public string DocMasName { get; set; }

        [Required(ErrorMessage = "Select pdf file")]
        [Display(Name = "Upload files")]
        public HttpPostedFileBase newpdfFile { set; get; }

        [Display(Name = "Upload PDF")]
        public string Filename { set; get; }

        [Required(ErrorMessage = "Enter final score")]
        [Display(Name = "Enter score")]
        public decimal decFinalScore { get; set; }

        //[Required(ErrorMessage = "Select year")]
        public string intYear { get; set; }
    }
}
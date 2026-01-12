using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class UpMOUViewModel
    {
        public int MOUid { get; set; }
        public int empID { get; set; }

        [Display(Name = "Mobile")]
        public string mobile { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Temp. Code")]
        public string EmpTcode { get; set; }

        [Display(Name = "MOU Date from")]
        [Required(ErrorMessage = "Select MOU date effect from")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:d}"))]
        public DateTime dtEffectFrom { get; set; }

        [Display(Name = "MOU Date To ")]
        [Required(ErrorMessage = "Select MOU date effect to")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:d}"))]
        public DateTime dtEffectTo { get; set; }

        [Display(Name = "MOU Date")]
        [Required(ErrorMessage = "Select MOU date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:d}"))]
        public DateTime dtCreatedMOU { get; set; }

        [Display(Name = "File name")]
        public string fileName { set; get; }

        [Display(Name = "Upload files")]
        [Required(ErrorMessage = "Select pdf file")]
        public HttpPostedFileBase pdfFile { set; get; }
    }
}
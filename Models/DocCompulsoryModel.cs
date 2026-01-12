using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc; 

namespace HRM.Models
{
    public class DocCompulsoryModel
    {
        public IEnumerable<SelectListItem> compdoc { set; get; }


        [Required(ErrorMessage = "Select compulsory doc file")]
        [Display(Name = "Upload compulsory doc")]
        public HttpPostedFileBase compdocument { set; get; }

        [Display(Name = "Upload PDF")]
        [Required(ErrorMessage = "Select file")]
        public string compdocname { set; get; }

        public int empid { set; get; }
        public Boolean BitIsCompleted { set; get; }
    }
}
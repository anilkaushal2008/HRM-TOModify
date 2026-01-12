using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class ProfileViewModel
    {


        [Required(ErrorMessage = "Select photograh file")]
        [Display(Name = "Photograph")]
        public HttpPostedFileBase profilepic { set; get; }

        [Required(ErrorMessage = "Enter file name")]
        public string picname { set; get; }
        public Boolean BitIsCompleted { set; get; }
        public int docid { set; get; }
        public int empid { set; get; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class ChangeProfile
    {
        public int id { get; set; }

        [Required(ErrorMessage ="Enter file name")]
        public string filename { get; set; }
        [Required(ErrorMessage ="Select jpg file")]
        public HttpPostedFileBase filepath { get; set; }

    }
}
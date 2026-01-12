using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class AssDeactiveViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage ="Please enter remarks")]
        [Display(Name ="Remarks")]
        public string Remarks { get; set; }
    }
}
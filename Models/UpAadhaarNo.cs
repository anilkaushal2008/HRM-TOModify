using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class UpAadhaarNo
    {
        public int empid { get; set; }

        public string name { get; set; }

        [Required(ErrorMessage = "Aadhaar number is required")]

        [RegularExpression(@"^\d{4}-\d{4}-\d{4}$", ErrorMessage = "Invalid Aadhaar number format.")]
        public string aadharno { get; set; }

    }
}
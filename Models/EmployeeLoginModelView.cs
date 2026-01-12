using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class EmployeeLoginModelView
    {
        [Display(Name ="OTP")]
        [Required(ErrorMessage ="Enter OTP")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "OTP must be exactly 4 digits.")]
        public int OTP { get; set; }

        [Display(Name ="Mobile number")]
        [Required(ErrorMessage ="Enter mobile number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be exactly 10 digits.")]
        public string vchmobile { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class EmpLoginMasMetaData
    {
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        [Required(ErrorMessage ="Please enter mobile number")]
        [Display(Name ="Mobile number")]
        public string vchmobile;

        [Required(ErrorMessage = "Please enter OTP")]
        [Display(Name ="OTP")]
        public int? vchOTP;
    }
}
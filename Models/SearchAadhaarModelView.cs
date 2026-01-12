using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class SearchAadhaarModelView
    {
        [Required(ErrorMessage ="Enter aadhaar number for searching")]
        [RegularExpression(@"^\d{4}-\d{4}-\d{4}$", ErrorMessage = "Invalid Aadhaar number format.")]
        public string aadhaarno { get; set; }

    }
}
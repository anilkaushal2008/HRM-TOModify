using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class DateRemarksViewModel
    {
        public int id { get; set; }

        [Display(Name = "Remarks")]
        [Required(ErrorMessage = "Remarks is required.")]
        [StringLength(2000, ErrorMessage = "Character length should be equal to or less than 2000 characters")]
        public string dateRemarks { get; set; }
    }
}
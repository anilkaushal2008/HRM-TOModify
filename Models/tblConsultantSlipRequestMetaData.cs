using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class tblConsultantSlipRequestMetaData
    {
        [Required(ErrorMessage = "Select quarter")]
        public int fk_Quarter { get; set; }

        //[Required(ErrorMessage = "Professional Fee is required")]
        //[Range(1, double.MaxValue, ErrorMessage = "Fee must be greater than zero")]
        //public decimal decAmount { get; set; }
    }
}
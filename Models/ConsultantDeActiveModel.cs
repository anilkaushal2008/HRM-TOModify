using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class ConsultantDeActiveModel
    {

        public int ConsultantId { get; set; }
        [Display(Name="Name")]
        public string Name { get; set; }

        [Display(Name="Employee Code")]
        public string EmpCode { get; set; }

        [Display(Name ="Aadhaar No.")]
        [Required(ErrorMessage = "Enter aadhaar no")]
        public string AadharNo { get; set; }
        [Required(ErrorMessage ="Select DOL")]
        [Display(Name="Enter DOL")]
        public System.DateTime dol { get; set; }

        [Display(Name ="Remarks")]
        public string remarks { get; set; }

    }
}
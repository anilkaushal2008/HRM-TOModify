using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace HRM.Models
{
    public class EmpFinalUpdateViewModel
    {
        public int id { get; set; }
        public string tcode { get; set; }

        [Display(Name = "Expected D.O.J (DD/MM/YYYY)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime Edatejoin { get; set; }
        public string fcode { get; set; }

        [Required(ErrorMessage = "Select Final DOJ (DD/MM/YYYY)")]

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime dtDOJ { get; set; }

        [Required(ErrorMessage = "Please select Employee type")]
        public Boolean bitIsCorporateemp { get; set; }
        public Boolean bitIsUnitEmp { get; set; }
    }
}
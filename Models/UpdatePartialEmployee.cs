using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class UpdatePartialEmployee
    {
        public int empid { get; set; }

        [Display(Name = "Selected Employee Code")]
        public string fcode { get; set; }

        [Display(Name = "Selected Employee Temp Code")]
        public string tcode { get; set; }

        [Display(Name = "Selected Employee Name")]
        public string empname { get; set; }

        [Display(Name ="Father Name")]
        public string FatherName { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:dd/MM/yyyy}"))]
        [Display(Name ="D.O.J (DD/MM/YYYY)")]
        public DateTime DOJ { get; set; }


     
    }
}
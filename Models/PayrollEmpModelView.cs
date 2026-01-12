using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class PayrollEmpModelView
    {
        //Master table entry
        //public string string_id { get; set; }

        [Required(ErrorMessage ="Enter salary")]
        [Display(Name ="Salary")]
        public string salary { get; set; }

        [Required(ErrorMessage ="Enter employee name")]
        [Display(Name ="Employee name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Select position")]

        [Display(Name = "Position")]
        public int fk_position { get; set; }

        //[Required(ErrorMessage = "Enter date of joining")]
        [Display(Name = "D.O.J")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:d}"))]
        public string DOJ { get; set; }

        //Details table entry
        [Required(ErrorMessage = "Mobile number required")]
        [Display(Name = "Enter mobile number")]
        public string vchMobile { get; set; }

        [Required(ErrorMessage = "Employee old code required")]
        [Display(Name = "Enter employee old code")]
        public string vchOldCode { get; set; }

        [Required(ErrorMessage = "Gender required")]
        [Display(Name = "Gender")]
        public string vchgender { get; set; }

        [Display(Name = "Department Name")]
        [Required(ErrorMessage = "Select department")]
        public int fk_deptid { get; set; }

        [Display(Name = "Designation")]
        [Required(ErrorMessage ="Select designation")]
        public int fk_desiid { get; set; }

        [Required(ErrorMessage = "Select Employee type")]
        public Boolean bitIsCorporateemp { get; set; }

        [Required(ErrorMessage ="Select title")]
        [Display(Name ="Title")]
        public int fk_titid { get; set; }

        [Required(ErrorMessage = "Select title")]
        [Display(Name = "Branch")]
        public int fkBranch { get; set; }

        [Display(Name="Aadhaar No.")]
        public string AadharNo { get; set; }


    }
}
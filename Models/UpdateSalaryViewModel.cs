using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class UpdateSalaryViewModel
    {
        public int empid { get; set; }

        [Display(Name ="Old Salary")]
        public int salary { get; set; }

        [Display(Name ="Enter new salary")]
        [Required(ErrorMessage ="Please enter new salary")]
        public int newsalary { get; set; }

    }
}
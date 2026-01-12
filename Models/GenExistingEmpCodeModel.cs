using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc; 

namespace HRM.Models
{
    public class GenExistingEmpCodeModel
    {
        public int empid { get; set; }
        public string EmployeeName { get; set; }
        public IEnumerable<SelectListItem> AllEmployee { get; set; }

        public IEnumerable<string> SelectedEmployee { get; set; }

        [Required(ErrorMessage ="Select employee type")]
        public Boolean bitIsCorporateemp { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc; 

namespace HRM.Models
{
    public class NewMapingViewModel
    {
        public int id { get; set; }
        [Display(Name = "Select Position")]
        [Required(ErrorMessage = "Select position")]
        public string PositionName { get; set; }
       
        public IEnumerable<SelectListItem> AllPosition { get; set; }

        public List<SelectListItem> AllDesignation { get; set; }

        [Display(Name = "Select multiple designations")]
        [Required(ErrorMessage = "Select atleast one designation")]
        public IEnumerable<string> SelectedDesignation { get; set; }

        [Display(Name = "Is Last Assessment")]
        public bool bitIsAllowLastAss { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc; 

namespace HRM.Models
{
    public class MasQualificationViewModel
    {       
        public int id { get; set; }
        public IEnumerable<SelectListItem> Qualifications { get; set; }
        [Required(ErrorMessage ="Select atleast one qualification")]
        public IEnumerable<string> SelectedQualifications { get; set; }
        public IEnumerable<SelectListItem> AllQualification { get; set; }      

        [Required(ErrorMessage ="Select final submission")]
        public bool bitQualification { get; set; }

        [Required(ErrorMessage ="Select employee temp code")]
        [Display(Name ="Select Emp Code")]
        public string Tempcode { get; set; }

    }
}
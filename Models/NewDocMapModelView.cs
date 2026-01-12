using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc; 

namespace HRM.Models
{
    public class NewDocMapModelView
    {
        public int id { get; set; }
        [Display(Name = "Select Position")]
        [Required(ErrorMessage = "Select position")]
        public string PositionName { get; set; }

        public IEnumerable<SelectListItem> AllPosition { get; set; }

        public List<SelectListItem> AllDocument { get; set; }
        public List<SelectListItem> AllForAuthor { get; set; }
        public List<SelectListItem> AllForComplete { get; set; }

        [Display(Name = "Select multiple document")]

        [Required(ErrorMessage = "Select atleast one document")]
        public IEnumerable<string> SelectedDocument { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc; 

namespace HRM.Models
{
    public class EditEmpTitleViewModel
    {
        [Required(ErrorMessage ="Select atleast one title")]
        public IEnumerable<SelectListItem> AllTitle { get; set; }
        public List<string> selecttitile { get; set; }
        
    }
}
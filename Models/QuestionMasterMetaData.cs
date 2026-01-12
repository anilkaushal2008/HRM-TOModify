using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class QuestionMasterMetaData
    {
        [Display(Name ="Question")]
        [Required(ErrorMessage ="Enter question")]
        public string vchQuestion { get; set; }

        [Display(Name = "Answer type")]
        [Required(ErrorMessage = "Select answer type")]
        public string vchAnsType { get; set; }

        public string vchcreatedby { get; set; }

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]     
        public string dtcreated { get; set; }
    }
}
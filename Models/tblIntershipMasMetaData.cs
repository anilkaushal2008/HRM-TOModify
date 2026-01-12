using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class tblIntershipMasMetaData
    {
        [Display(Name ="Master name")]
        [Required(ErrorMessage ="Select master name")]
        public string vchMasName { get; set; }

        [Display(Name = "Master content")]
        [Required(ErrorMessage = "Enter master content")]
        public string txtMasContent { get; set; }

        [Display(Name = "Master code")]
        [Required(ErrorMessage = "Enter master code")]
        public string vchLetterCode { get; set; }

        [Display(Name = "Letter heading")]
        [Required(ErrorMessage = "Enter letter heading")]
        public string vchLetterHeading { get; set; }

        [Display(Name = "Select for gender")]
        [Required(ErrorMessage = "Select gender")]
        public string vchForGender { get; set; }

        [Display(Name = "Creation date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:dd/MM/yyyy}"))]
        public System.DateTime dtCreated { get; set; }
    }
}
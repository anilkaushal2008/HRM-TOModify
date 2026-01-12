using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class tblAppointMasMetaData
    {
        [Display(Name ="Master type")]
        [Required(ErrorMessage ="Select master type")]
        public string vchMasType { get; set; }

        [Display(Name = "Master name")]
        [Required(ErrorMessage = "Enter master name")]
        public string vchMasName { get; set; }

        [Display(Name = "Master content")]
        [Required(ErrorMessage = "Enter master content")]
        public string TextMasContent { get; set; }

        [Display(Name = "Letter code")]
        [Required(ErrorMessage = "Enter letter code")]
        public string vchMasLetterCode { get; set; }

        [Display(Name = "Master heading")]
        [Required(ErrorMessage = "Enter master heading")]
        public string vchMasHeading { get; set; }

        [Display(Name = "Master for")]
        [Required(ErrorMessage = "Select master for")]
        public string vchForGender { get; set; }

        [Display(Name = "Creation date")]        
        public System.DateTime dtCreated { get; set; }
        
    }
}
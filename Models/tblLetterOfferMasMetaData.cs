using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class tblLetterOfferMasMetaData
    {
        [Display(Name ="Master name")]
        [Required(ErrorMessage ="Enter master name")]
        public string vchName { get; set; }

        [Display(Name ="Master type")]
        [Required(ErrorMessage ="Select offer master type")]
        public string vchMasType { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Select gender")]
        public string vchForGender { get; set; }

        [Display(Name = "Master Heading")]
        [Required(ErrorMessage = "Enter master heading")]
        public string vchMasHeading { get; set; }

        [Display(Name = "Content")]
        [Required(ErrorMessage = "Enter content")]
        public string txtMasContent { get; set; }

        [Display(Name = "Master code")]
        [Required(ErrorMessage = "Enter master code")]
        public string vchLetterCode { get; set; }

        [Display(Name = "Creation date")]
        public System.DateTime dtCreated { get; set; }
    }
}
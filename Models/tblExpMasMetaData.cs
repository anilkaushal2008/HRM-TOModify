using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class tblExpMasMetaData
    {

        [Display(Name ="Master name")]
        [Required(ErrorMessage = "Enter master name")]
        public string vchName { get; set; }
        [Display(Name = "Master type")]
        [Required(ErrorMessage = "Select master type")]
        public string vchType { get; set; }

        [Display(Name = "For gender")]
        [Required(ErrorMessage = "Select gender")]
        public string vchForGender { get; set; }

        [Display(Name = "Letter heading")]
        [Required(ErrorMessage = "Enter letter heading")]
        public string vchHeading { get; set; }

        [Display(Name = "Content")]
        [Required(ErrorMessage ="Enter letter content")]
        public string txtContent { get; set; }

        [Display(Name = "Letter Code")]

        [Required(ErrorMessage ="Enter letter code")]
        public string vchLetterCode { get; set; }

        [Display(Name ="Creation date")]
        public System.DateTime dtCreated { get; set; }

    }
}
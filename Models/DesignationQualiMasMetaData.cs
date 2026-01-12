using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace HRM.Models
{
    public class DesignationQualiMasMetaData
    {

        [Required(ErrorMessage ="Select designation")]
        public string fk_desiid;
        
        [Display(Name = "Enter qualification name")]
        [Required(ErrorMessage ="Enter qualification name")]
        public string vchQualification;

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:d}"))]
        public System.DateTime dtcreated;
    }
}
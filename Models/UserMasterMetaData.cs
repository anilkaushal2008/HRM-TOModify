using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class UserMasterMetaData
    {
        [Display(Name ="User Name")]
        [Required(ErrorMessage ="Enter user name")]
        public string vchUsername;

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Enter Password")]
        public string Passcode;

        [Display(Name ="CreatedBy")]
        public string vchcreatedby;
        
        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public System.DateTime dtcreated;
    }
}
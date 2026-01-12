using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class tblDocMasMetaData
    {
        [Required(ErrorMessage ="Enter document name")]
        [Display(Name ="Document Name")]
        public string vchdocname;

        [Display(Name = "Created By")]
        public string vchcreatedby;

        [Display(Name = "Creation Date")]
        [DisplayFormat(ApplyFormatInEditMode =true,DataFormatString = ("{0:d}"))]
        public System.DateTime dtcreated;
    }
}
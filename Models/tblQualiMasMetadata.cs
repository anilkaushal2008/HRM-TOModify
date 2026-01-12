using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace HRM.Models
{
    public class tblQualiMasMetaData
    {
        [Required(ErrorMessage = "Select/enter qualification name ")]
        [Display(Name = "Qualifiction/Doc name")]
        public string vchqname;

        [Display(Name = "Created By")]
        public string vchcreatedby;

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString ="{0:d}",ApplyFormatInEditMode =true)]
        public System.DateTime dtcreated;

    }
}
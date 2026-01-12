using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class tblTitleMetaData
    {
        [Required(ErrorMessage ="Select/Enter title name")]
        [Display(Name ="Title Name")]
        public string vchname;

        [Display(Name ="Created By")]
        public string vchcreatedby;

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public System.DateTime dtcreated;
    }
}
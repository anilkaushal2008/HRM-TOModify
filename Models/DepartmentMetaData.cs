using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class tblDeptMasMetaData
    {
        [Required(ErrorMessage ="Enter/Select department name")]
        [Display(Name ="Department Name")]
        public string vchdeptname;

        //[Required(ErrorMessage = "Enter mapower counter")]
        //[Display(Name = "Manpower Count")]
        //public string intManPower;

        [Display(Name = "Created By")]
        public string vchcreatedby;

        [Display(Name = "Creation Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:d}"))]
        public System.DateTime dtcreated;
    }
}
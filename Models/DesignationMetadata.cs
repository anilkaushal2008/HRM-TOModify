using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class tblDesignationMasMetadata
    {
        [Required(ErrorMessage = "Select Department")]
        [Display(Name = "Select Department")]
        public string intdeptid;

        [Required(ErrorMessage ="Enter/Select designation name ")]
        [Display(Name ="Designation Name")]
        public string vchdesignation;

        [Display(Name = "Created By")]
        public string vchcreatedby;

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:d}"))]
        public System.DateTime dtcreated;


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class GroupMasterMetaData
    {
        [Display(Name ="Group Name")]
        [Required(ErrorMessage ="Enter Group Name")]
        public string vchGpName;

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public System.DateTime dtCreated;

        [Display(Name = "CreatedBy")]
        public string vchCreatedBy;
    }
}
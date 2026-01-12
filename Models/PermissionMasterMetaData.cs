using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class PermissionMasterMetaData
    {
        [Display(Name = "GroupName")]
        [Required(ErrorMessage = "Select Group name")]
        public string fk_GName;

        [Required(ErrorMessage = "Please enter permission name")]
        [Display(Name = "Permission Name")]
        public string vchPermissionName;

        [Display(Name="CreatedBy")]
        public string vchCreatedBy;

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public System.DateTime dtCreated;
    }
}
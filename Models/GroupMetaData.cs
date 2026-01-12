using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class GroupMetaData
    {
        [Required(ErrorMessage ="Select an Company")]
        [Display(Name="Select Company")]
        public int intPK { get; set; }
     
        public string descript { get; set; }

    }
}
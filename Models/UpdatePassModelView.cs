using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class UpdatePassModelView
    {
       public int userid { set; get; }

       [Display(Name ="Enter old password")]
       public string oldpasscode { get; set; }

       [Display(Name ="Enter new password")]
       public string newpasscode1 { get; set; }
       
       [Display(Name ="Confirm password")]
       public string newpasscode2 { get; set; }
    }
}
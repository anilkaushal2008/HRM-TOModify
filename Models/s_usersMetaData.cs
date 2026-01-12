using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class s_usersMetaData
    {
        [Display(Name ="User Name")]
        [Required(ErrorMessage ="Enter user name")]
        public string descript;

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Enter password")]
        public string passwrd; 
    }
}
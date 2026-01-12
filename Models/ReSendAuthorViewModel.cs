using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class ReSendAuthorViewModel
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Select Yes/No")]
        public Boolean bitAuthorBack { get; set; }

        [Required(ErrorMessage = "Please enter message to authorizer")]
        [StringLength(2000,ErrorMessage ="Character length should be equal to or less than 2000 characters")]
        public string vchHrSolRemark { get; set; }
    }
}
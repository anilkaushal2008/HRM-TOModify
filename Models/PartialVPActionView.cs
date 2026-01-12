using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class PartialVPActionView
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Please select Yes/No")]
        public string bitIsAuthorised { get; set; }

        [StringLength(2000, ErrorMessage = "Character length should be equal to or less than 2000 characters")]
        [Required(ErrorMessage = "Please enter autorization remarks")]
        public string vchRemarks { get; set; }

        public int salary { get; set; }
    }
}
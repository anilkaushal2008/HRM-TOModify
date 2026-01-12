using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class RequisitionAuthoriseModel
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Enter remarks")]
        public string vchRemarks { get; set; }

        [Required(ErrorMessage = "Select an option")]
        public string bitAction { get; set; }

        [Required(ErrorMessage ="Select approved date")]
        public System.DateTime dtApproved { get; set; }
    } 
}
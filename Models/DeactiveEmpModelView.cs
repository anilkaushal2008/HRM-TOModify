using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.Ajax.Utilities;

namespace HRM.Models
{
    public class DeactiveEmpModelView
    {
        public int empid { get; set; }

        [Display(Name="Selected Employee Code")]
        public string fcode { get; set; }

        [Display(Name = "Selected Employee Name")]
        public string empname { get; set; }

        [Display(Name ="Select Date Of Leaving")]
        [Required(ErrorMessage = "Please enter a date of leaving")]       
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public System.DateTime dol { get; set; }

        [Display(Name ="Enter remarks")]
        [Required(ErrorMessage ="Enter remarks")]
        [StringLength(2000, ErrorMessage = "Character length should be equal to or less than 2000 characters")]    
        public string vchRemarks {  get; set; }

        [Display(Name ="Aadhar Card Number")]
        [Required(ErrorMessage ="Enter aadhar card number")]
        [RegularExpression(@"^\d{4}-\d{4}-\d{4}$", ErrorMessage = "Invalid Aadhaar number format, valid format as(XXXX-XXXX-XXXX)")]
        public string vchAadharNo { get; set; }

        [Display(Name ="Is Flagging Employee")]
        [Required(ErrorMessage ="Please select a flag for employee")]
        public string BitIsFlaggingEmp { get; set; }

        [Display(Name = "Select status Employee")]
        [Required(ErrorMessage = "Select a status for employee")]
        public string Empstatus { get; set; }

        


    }
}
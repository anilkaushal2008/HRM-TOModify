using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class EmpTransferModelView
    {
        public int empid { get; set; }

        [Display(Name = "Employee Name")]
        public string empname { get; set; }

        [Display(Name = "Employee Code")]
        public string empcode { get; set; }
        public int transferSalary { get; set; }

        [Display(Name = "Relieving date")]

        [Required(ErrorMessage ="Select DOL")]       
        public DateTime DOL { get; set; }

        [Display(Name = "Remarks")]
        [Required(ErrorMessage = "Enter remarks")]
        [StringLength(2000, ErrorMessage = "Character length should be equal to or less than 2000 characters")]
        public string tremarks { get; set; }

        [Display(Name ="Transfer to branch")]
        [Required(ErrorMessage ="Select transfer to branch")]
        [Range(1,int.MaxValue,ErrorMessage = "Select transfer branch ")]
        public int  ToTransferBranch { get; set; }

        [Display(Name ="Aadhaar No")]
        [Required(ErrorMessage ="Enter aadhaar number")]
        [RegularExpression(@"^\d{4}-\d{4}-\d{4}$", ErrorMessage = "Invalid Aadhaar number format.")]
        public string  AadharNo { get; set; }
        
       
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class AssesmentMasMetaDeta
    {
        //[Display(Name = "Employee name")]
        [Required(ErrorMessage = "Please select employee replacement")]
        public Boolean bitIsReplacement;

        [Display(Name ="Employee name")]
        [Required(ErrorMessage ="Enter employee name")]
        public string vchName;

        [Display(Name = "Employee mobile")]
        [Required(ErrorMessage = "Enter employee mobile")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public string vchMobile;

        [Display(Name = "Select position")]
        [Required(ErrorMessage = "Position should not b blank")]
        public int fk_PositionId;

        [Display(Name ="AssignBy")]
        public string vchAssignedBy;

        [Display(Name ="Assign date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:d}"))]
        public DateTime dtAssign { get; set; }

        [Display(Name = "Created date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:d}"))]
        public DateTime dtcreated { get; set; }

        [Display(Name = "CreatedBy")]
        public string vchcreatedby { get; set; }


        [Display(Name ="Experience in year")]
        [Required(ErrorMessage ="Enter experience in year")]
        public decimal decExperience { set; get; }

        [Display(Name ="Working area/Profile")]
        [Required(ErrorMessage ="Enter working area")]
        public string vchWorkedArea { set; get; }
        [Display(Name ="EQ/IQ Marks")]
        //[Required(ErrorMessage ="Enter EQ/IQ Marks")]
        public decimal decSkillMarks;

        [Display(Name = "EQ/IQ Status")]
        public string vchSkillStatus;

        [Display(Name = "Expected DOJ")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:dd/MM/yyyy}"))]
        public DateTime dtDOJ { get; set; }

        [Display(Name = "D.O.J")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:dd/MM/yyyy}"))]
        public DateTime dtFDOJ { get; set; }

        [Display(Name = "D.O.L")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:dd/MM/yyyy}"))]
        public DateTime dtDOL { get; set; }

        [Display(Name ="Emp. Code")]
        public string vchEmpFcode { get; set; }

        //[Required(ErrorMessage ="Enter replacment name")]
        public string vchRplcmntName { get; set; }

       // [Required(ErrorMessage ="Select recruitment area")]
        public string vchArea { get; set; }

        [Display(Name ="Salary")]
        public string intsalary { get; set; }

        [Required(ErrorMessage ="Aadhaar number is required")]

        [RegularExpression(@"^\d{4}-\d{4}-\d{4}$", ErrorMessage = "Invalid Aadhaar number format.")]
        public string vchAadharNo { get; set; }
    }

}
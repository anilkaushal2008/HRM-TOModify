using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class ByPassEntryEmpModel
    {
        //mas table data

        [Required(ErrorMessage = "Select title name")]
        [Display(Name = "Select title")]
        public int fk_titid { get; set; }

        [Display(Name = "Enter name")]
        [Required(ErrorMessage = "Enter name")]
        public string vchname { get; set; }

        [Required(ErrorMessage = "Enter mobile number")]
        [Display(Name = "Mobile number")]
        public string vchmobile { get; set; }

        [Required(ErrorMessage = "Select position")]
        [Display(Name = "Select position")]
        public int fk_positionid { get; set; }

        [Required(ErrorMessage = "Select department")]
        [Display(Name = "Department")]
        public int fk_deptid { get; set; }

        [Required(ErrorMessage = "Select designation")]
        [Display(Name = "Designation")]
        public int fk_desiidi { get; set; }

        [Required(ErrorMessage ="Enter experience")]
        [Display(Name = "Experience")]
        public decimal experience { get; set; }

        [Required(ErrorMessage ="Enter worked area")]
        [Display(Name = "Worked area")]
        public string vchworkedarea { get; set; }

        [Required(ErrorMessage = "Enter salary")]
        [Display(Name = "Salary")]
        public int salary { get; set; }

        [Required(ErrorMessage = "Select is replacement or not")]
        public Boolean bitIsReplacement { get; set; }
        [Display(Name = "replacement")]
        public string vchRplcmntName { get; set; }

        [Display(Name = "Department")]
        [Required(ErrorMessage = "Select department")]
        public string DeptName { get; set; }

        [Display(Name = "Designation")]
        [Required(ErrorMessage = "select designation")]
        public string Designation { get; set; }

        //details table empdetail data
        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Select gender")]
        public string vchgender { get; set; }

        [Display(Name = "DOB")]
        [Required(ErrorMessage = "Select DOB")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime dtDOB { get; set; }

        [Display(Name = "DOJ")]
        [Required(ErrorMessage = "Select DOJ")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime dtDOJ { get; set; }

        [Display(Name = "age")]
        public int age { get; set; }

        [Display(Name = "Father name")]
        [Required(ErrorMessage = "Enter father name")]
        public string vchfathername { get; set; }

        [Display(Name = "Mother name")]
        [Required(ErrorMessage = "Enter mother name")]
        public string vahmothername { get; set; }

        [Display(Name = "Marital stutus")]
        [Required(ErrorMessage = "Marital status")]
        public string vchmaritalststus { get; set; }

        [Display(Name = "Spouse name")]
        public string vchspousename { get; set; }

        [Display(Name = "Nominee")]
        [Required(ErrorMessage = "Enter nominee")]
        public string vchNominee { get; set; }
        [Display(Name = "Relation")]
        [Required(ErrorMessage = "Enter relation")]
        public string vchNomRelation { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Enter address")]
        public string vchaddress { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Enter state")]
        public string vchstate { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "Enter city")]
        public string vchcity { get; set; }

        [Display(Name = "Pin Code")]
        [Required(ErrorMessage = "Enter pin code")]
        public string vchpincode { get; set; }
       
        [Required(ErrorMessage = "Select Employee type")]
        public bool? bitIsCorporateemp { get; set; }

        [Required(ErrorMessage = "Enter salary")]
        [Display(Name = "Salary")]
        public int intsalary { get; set; }

        [Required(ErrorMessage = "Aadhaar No")]
        [RegularExpression(@"^\d{4}-\d{4}-\d{4}$", ErrorMessage = "Invalid Aadhaar number format.")]
        public string aadhaarno { get; set; }
    }
}
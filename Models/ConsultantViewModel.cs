using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class ConsultantViewModel
    {
        [Display(Name ="Name")]
        [Required(ErrorMessage ="Enter consultant name")]
        public string Name { get; set; }

        [Display(Name ="Father name")]
        [Required(ErrorMessage ="Enter father name")]
        public string FatherName { get; set; }

        [Display(Name ="Mother name")]
        [Required(ErrorMessage = "Enter mother name")]
        public string MotherName { get; set; }

        [Display(Name ="Aadhaar No.")]
        [Required(ErrorMessage ="Enter aadhaar number")]
        [RegularExpression(@"^\d{4}-\d{4}-\d{4}$", ErrorMessage = "Invalid Aadhaar number format")]
        public string AadhaarNo { get; set; }

        [Display(Name = "Position")]     
        public int fk_Position { get; set; }

        [Display(Name ="Department")]
        [Required(ErrorMessage ="Select department")]
        public int fk_Dept { get; set; }

        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Select department")]
        public int fk_desi { get; set; }

        [Display(Name ="DOJ")]
        [Required(ErrorMessage ="Select date of joining")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:d}"))]
        public DateTime DOJ { get; set; }

        [Display(Name = "DOB")]
        [Required(ErrorMessage = "Select date of birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime DOB { get; set; }

        [Display(Name="Qualification")]
        [Required(ErrorMessage ="Enter qualifications")]
        public string Qualification { get; set; }

        [Display(Name ="Age")]
        public int age { get; set; }

        [Display(Name="Mobile")]
        [Required(ErrorMessage ="Enter mobile number")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number")]
        public string mobile { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Select gender")]
        public string vchGender { get; set; }

        [Display(Name = "Marital status")]
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

        //[Display(Name = "Address")]
        //[Required(ErrorMessage = "Enter address")]
        //public string vch_t_address { get; set; }

        //[Display(Name = "State")]
        //[Required(ErrorMessage = "Select state")]
        //public string vch_t_state { get; set; }

        //[Display(Name = "City")]
        //[Required(ErrorMessage = "Select city")]
        //public int vch_t_city { get; set; }

        //[Display(Name = "Pin Code")]
        //[Required(ErrorMessage = "Enter pin code")]
        //public string vch_t_pincode { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Enter address")]
        public string vchaddress { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Select state")]
        public string fk_state { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "Select city")]
        public int fk_city { get; set; }

        [Display(Name = "Pin Code")]
        [Required(ErrorMessage = "Enter pin code")]
        public string vchpincode { get; set; }        

        [Required(ErrorMessage = "Enter salary")]
        [Display(Name = "Salary")]
        public int intsalary { get; set; }

        [Required(ErrorMessage = "Enter experience")]
        [Display(Name = "Experience")]
        public decimal experience { get; set; }

        [Display(Name = "Type")]
        [Required(ErrorMessage = "Select type")]
        public string type { get; set; }

    }
}
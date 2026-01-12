using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class EmployeeDetailsMetaData
    {
        [Display(Name = "Title")]
        [Required(ErrorMessage = "Please select title")]
        public int fk_titid;

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please enter first name")]
        public string vchfname;

        [Display(Name = "Middle Name")]
        public string vchmname;

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Please enter last name")]
        public string vchlname;

        [Display(Name ="Gender")]
        [Required(ErrorMessage ="Please select gender")]
        public string vchsex;

        [Display(Name = "Marital status")]
        [Required(ErrorMessage = "Select Marital Status")]
        public string vchmaritalststus;

        [Display(Name = "D.O.B")]
        [Required(ErrorMessage = "Enter D.O.B")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime dtDob;

        [Display(Name = "Age")]
        [Required(ErrorMessage ="Enter age")]
        public int intage;

        [Display(Name = "Spouse name")]
        public string vchspouse;

        [Display(Name = "Mother's name")]
        [Required(ErrorMessage = "Enter mother name")]
        public string vchmothername;

        [Display(Name = "Father's name")]
        [Required(ErrorMessage = "Enter father name")]
        public string vchFatherName;

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Enter address")]
        public string vchtaddress;

        [Display(Name = "City")]
        [Required(ErrorMessage = "Select city")]
        public string vchtcity;

        [Display(Name = "State")]
        [Required(ErrorMessage = "Select state")]
        public string vchtstate;

        [Display(Name = "Pincode")]
        [Required(ErrorMessage = "Enter pincode")]
        public int inttpin;

        [Display(Name = "Mobile")]
        [Required(ErrorMessage = "Enter mobile no.")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public int vchtmobile;

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Enter address")]
        public string vchpaddress;

        [Display(Name = "City")]
        [Required(ErrorMessage = "Select city")]
        public string vchpcity;

        [Display(Name = "State")]      
        [Required(ErrorMessage = "Select state")]
        public string vchpstate;

        [Display(Name = "Pincode")]
        [Required(ErrorMessage = "Please enter pincode")]
        public int intppin;

        [Display(Name = "Mobile")]
        [Required(ErrorMessage = "Enter mobile no.")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public int vchpmobile;

        [Display(Name = "Is final completion")]
        [Required(ErrorMessage ="Please select final completion")]
        public bool BitCompleted;
    }
}
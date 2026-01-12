using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc; 
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class UserPermissionViewModel
    {
        //permission master data
  
        [Display(Name = "Permission Name")]
        public string pname { get; set; }

        [Display(Name = "Group Name")]
        public string gpname { get; set; }

        public List<SelectListItem> AllPermission { get; set; }
        public List<SelectListItem> AllGpname { get; set; }

        //jquery used for validation
        [Display(Name = "User Name")]
        public string Username { get; set; }

        [Display(Name = "Department Name")]
        public string DeptName { get; set; }
        
        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [Display(Name ="Enter mobile number")]
        [DataType(DataType.PhoneNumber)]
        public string MobileNo { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required")]
        //[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string EmailAddess { get; set; }
            
        [Display(Name = "Allow")]
        public List<bool> selectedpermission { get; set; }

        public List<string> allusers { get; set; }

        [Required(ErrorMessage ="Enter Salary range from")]
        [Display(Name ="Salary from")]
        public int salaryfrom { get; set; }

        [Required(ErrorMessage = "Enter Salary range to")]
        [Display(Name = "Salary to")]
        public int salaryto { set; get; }

        public bool bitSalCheck { get; set; }

        [Display(Name="Allow employee assesment")]
        public bool bitAllowAssesment { get; set; }

        [Display(Name ="Password")]
        public string Passcode { get; set; }

        [Display(Name = "Active/Deactive")]
        public bool bitActive { get; set; }

        [Display(Name = "IsHOD?")]
        public bool bitISHOD { get; set; }



    }
}
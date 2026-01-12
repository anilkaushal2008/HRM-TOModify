using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class tblLetterOfferDetailMetaData
    {

        [Display(Name = "Offer master")]
        [Required(ErrorMessage = "Select master")]
        public int fk_OfferMas_id { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Select gender")]
        public string vchGender { get; set; }

        [Display(Name = "Department")]
        [Required(ErrorMessage = "Select department")]
        public int fk_department { get; set; }

        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Select designation")]
        public int fk_designation { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Enter name")]
        public string vchName { get; set; }

        [Display(Name = "Father name")]
        [Required(ErrorMessage = "Enter father name")]
        public string vchFatherName { get; set; }

        [Display(Name = "Content")]
        [Required(ErrorMessage = "Letter content")]
        public string txtContent { get; set; }

        [Display(Name ="Letter date")]
        [Required(ErrorMessage ="Enter letter date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime dtAppdate { get; set; }

        [Display(Name = "Acceptance date")]
        [Required(ErrorMessage = "Enter ecceptance date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime dtAcceptdate { get; set; }

        [Display(Name = "Joining date")]
        [Required(ErrorMessage = "Enter non-ecceptance date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime dtDOJ { get; set; }      
     
        [Display(Name = "Emp Code")]
        public string vchEmpCode { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "Enter state")]
        public string vchState { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "Enter city")]
        public string vchCity { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Enter address")]
        public string vchAddress { get; set; }     

        [Display(Name = "Select employee code")]
        public Nullable<int> fk_HRMS_id { get; set; }
        [Required(ErrorMessage = "Select one option")]
        public bool bitIshMRMSemp { get; set; }

        [Display(Name = "Creation Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:dd/MM/yyyy}"))]
        public System.DateTime dtCreated { get; set; }
    }
}
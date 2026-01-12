using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc; 

namespace HRM.Models
{
    public class OfficialDetailsModelView
    {
        public int Empid { get; set; }

        [Required(ErrorMessage = "Expected date of joining")]
        [Display(Name = "DOJ")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:dd/MM/yyyy}"))]
        public System.DateTime dtDOJ { get; set; }

        [Required(ErrorMessage = "Enter salary")]
        [Display(Name = "Salary")]
        public int intsalary { get; set; }

        [Required(ErrorMessage = "Select Yes/No")]
        public bool bitoffdetail { get; set; }

       
        [Display(Name = "Department")]
        public string fk_dptid { get; set; }

        [Required(ErrorMessage = "Select Designation")]
        [Display(Name = "Designation")]
        public int fk_desiid { get; set; }
        public virtual tblDeptMas tblDeptMas { get; set; }
        public virtual tblDesignationMas tblDesignationMas { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc; 

namespace HRM.Models
{
    public class FillAssesmentModelView
    {
     
        public int aempid { get; set; }

        [Display(Name ="Employee name")]
        public string empname { get; set; }

        [Display(Name ="Mobile number")]
        public string mobileno { get; set; }
        
        [Display(Name ="Hiring for position")]
        public string position { get; set; }
        public string positionid { get; set; }
        [Display(Name ="Work experience in year")]
        public string decexp { get; set; }
        [Display(Name ="Working Domain/Area")]
        public string workdomain { get; set; }
        public int quid { get; set; }
        
        //[Display(Name ="Question")]
        //public string vchquestion { get; set; }
       // public string vchanswer { get; set; } 
        public IEnumerable<SelectListItem> allquestion { get; set; }
        
    }
}
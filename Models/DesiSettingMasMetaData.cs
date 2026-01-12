using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class DesiSettingMasMetaData
    {
        [Required(ErrorMessage ="Select designation")]
        public int fk_intdesiid;

        [Display(Name ="Enter minimum experience")]
        [Required(ErrorMessage ="Enter minimum experience")]
        public decimal numExpMin;

        [Display(Name = "Enter maximum experience")]
        [Required(ErrorMessage = "Enter maximum experience")]
        public decimal numExpMax;

        [Display(Name = "Enter minimum salary")]
        [Required(ErrorMessage = "Enter minimum salary")]
        public decimal numSalMin;

        [Display(Name = "Enter maximum salary")]
        [Required(ErrorMessage = "Enter maximum salary")]
        public decimal numSalMax;

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public System.DateTime dtcreated; 

    }
}
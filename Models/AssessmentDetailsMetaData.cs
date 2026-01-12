using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class AssessmentDetailsMetaData
    {
        [Display(Name = "CompletedDate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:dd/MM/yyyy}"))]
        public DateTime dtCompleted { get; set; }
    }
}
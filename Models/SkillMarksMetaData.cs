using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class SkillMarksMetaData
    {
       
        public int? fk_PositionID;
        
        [Required(ErrorMessage ="Skill marks should not be blank")]
        [Display(Name ="Enter skill marks range from")]
        public int? intSkillMarksFm;

        [Required(ErrorMessage = "Skill marks should not be blank")]
        [Display(Name ="Enter skill marks range from")]
        public int? intSkillMarksTo;

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public System.DateTime dtCreated;
    }
}
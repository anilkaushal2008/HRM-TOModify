using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
   
    public class tblRegistrationMetaData
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:d}"))]
        public System.DateTime dtRegistration { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:d}"))]
        public System.DateTime dtRegistrationFrom { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:d}"))]
        public System.DateTime dtRegistrationTo { get; set; }
    }
}
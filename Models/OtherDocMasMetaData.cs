using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class OtherDocMasMetaData
    {
        [Required(ErrorMessage ="Enter document name")]
        [Display(Name ="Document name")]
        public string vchDocName { get; set; }

       
        [Display(Name = "Allow multiple uploads")]
        public bool bitISMultipleRecords { get; set; }

        [Display(Name = "Creation Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public System.DateTime dtCretaed { get; set; }
    }
}
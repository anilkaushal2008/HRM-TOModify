using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class tblCurrentExpMetaData
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:dd/MMMM/yyyy}"))]
        public Nullable<System.DateTime> dtdoj { get; set; }

        [Required(ErrorMessage ="Enter Father Name")]
        public string vchFatherName { get; set; }

        [Required(ErrorMessage = "Enter Address")]
        public string vchAddress { get; set; }
    }
}
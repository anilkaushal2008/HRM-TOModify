using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class tblPayrollMetaData
    {
        public int intid { get; set; }
        public string Name { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime dtDOJ { get; set; }
        public string vchMobile { get; set; }
        public string vchEMpCode { get; set; }
        public string vchGender { get; set; }
        public string vchFatherName { get; set; }
        public string vchMotherName { get; set; }
        public string vchSpouseName { get; set; }
        public int intCode { get; set; }
        public bool BitInHRMS { get; set; }
    }
}
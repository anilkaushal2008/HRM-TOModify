using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class PosDocMapEditViewModel
    {
        public int intdocid { get; set; } 
        public string docname { get; set; } 
        public Boolean IsSelected { get; set; } 
        public Boolean RequiredForAuthor { get; set; } 
        public Boolean RequiredForComplete { get; set; } 
    }
}
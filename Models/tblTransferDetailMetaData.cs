using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class tblTransferDetailMetaData
    {

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:dd/MM/yyyy}"))]
        public Nullable<System.DateTime> dtRelieved { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:dd/MM/yyyy}"))]
        public Nullable<System.DateTime> dtTransferredDOJ { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:dd/MM/yyyy}"))]
        public Nullable<System.DateTime> dtTransfer { get; set; }
  

    }
}
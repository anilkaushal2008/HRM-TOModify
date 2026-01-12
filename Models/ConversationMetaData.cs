using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class ConversationMetaData
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:dd/MM/yyyy}"))]
        public DateTime dtMsgDate { get; set; }
    }
}
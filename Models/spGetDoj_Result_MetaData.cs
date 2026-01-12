using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Models
{
    public class spGetDoj_Result_MetaData
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:dd/MM/yyyy}"))]
        public System.DateTime dtDOJ { get; set; }

    }
}

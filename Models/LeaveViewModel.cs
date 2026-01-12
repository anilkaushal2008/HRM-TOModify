using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class LeaveViewModel
    {
        public tblLeaveApplication Master { get; set; }
        public tblLeaveApplicationDetail Detail { get; set; }
    }
}
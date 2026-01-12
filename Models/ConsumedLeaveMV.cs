using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class ConsumedLeaveMV
    {
        public string LeaveType { get; set; }
        public decimal ConsumedCount { get; set; }
        public decimal HalfYearLimit { get; set; }
        public bool IsHalfYearReached { get; set; }
        public bool bitAllowYearHalfCheck { get; set; }
    }
}
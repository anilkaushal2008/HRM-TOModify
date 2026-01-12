using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class UpdateDptMapModel
    {
        public int intCode { get; set; }
        public int textBoxValues { get; set; }
        public int FkDeptId { get; set; }
        public string branchName { get; set; }
        public string fkBranchID { get; set; }
    }
}
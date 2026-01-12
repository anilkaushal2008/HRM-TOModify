using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class DnbStudentListViewModel
    {
        public int EmployeeId { get; set; }
        public string EmpCode { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string DOJ { get; set; }
        //public string ProfilePicUrl { get; set; } //for profile image URL
        public int? DnbStudentId { get; set; }
        public string FeeStatus { get; set; }

    }
}
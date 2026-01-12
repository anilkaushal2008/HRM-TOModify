using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class DnbFeeStructurePageVM
    {
        public int StudentId { get; set; }
        public int EmployeeId { get; set; }
        public string StudentName { get; set; }
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<DnbFeeStructureVM> FeeDetails { get; set; } = new List<DnbFeeStructureVM>();
    }
    public class DnbFeeStructureVM
    {
        public int YearNumber { get; set; }
        public string PayableTo { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string PaymentStatus { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }
}
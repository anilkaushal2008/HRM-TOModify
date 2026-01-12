using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class DnbFeeDashboardVM
    {
        public int FeeStructureId { get; set; }
        public int DnbStudentId { get; set; }
        public int EmployeeId { get; set; }
        public int YearNumber { get; set; }
        public string PayableTo { get; set; }
        public decimal Amount { get; set; }
        public DateTime? DueDate { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentReferenceNo { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentMode { get; set; }
        public string SubmissionStatus { get; set; }
        public string VerifiedBy { get; set; }
        public string VerifiedRemarks { get; set; }
    }
}
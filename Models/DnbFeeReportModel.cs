using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class DnbFeeReportModel
    {
        public int DnbStudentId { get; set; }
        public int EmployeeId { get; set; }
        public string CourseName { get; set; }
        public int YearNumber { get; set; }
        public string PayableTo { get; set; }
        public decimal Amount { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentReferenceNo { get; set; }
        public string VerifiedBy { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string VerifiedRemarks { get; set; }
        public string VerificationStatus { get; set; }
        public string PaymentStatus { get; set; }
    }
}
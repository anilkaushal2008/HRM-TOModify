using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class DnbFeeVerificationViewModel
    {
        public string StudentName { get; set; }
        public int SubmissionId { get; set; }
        public string PaymentReferenceNo { get; set; }
        public string PaymentMode { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentScreenshotPath { get; set; }
        public string FileName { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string Status { get; set; }
        public string VerifiedBy { get; set; }
        public string VerifiedRemarks { get; set; }

        // Fee & Student Info
        public int FeeStructureId { get; set; }
        public int YearNumber { get; set; }
        public decimal Amount { get; set; }
        public string PayableTo { get; set; }
        public string CourseName { get; set; }
        public int EmployeeId { get; set; }
    }
}
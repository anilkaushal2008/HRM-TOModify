using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class PrintDNBSlipVM
    {
        public string vchName { get; set; }
        public string vchEmpFcode { get; set; }
        public string CourseName { get; set; }
        public decimal? Amount { get; set; }
        public string PayableTo { get; set; }
        public DateTime? DueDate { get; set; }
        public string PaymentReferenceNo { get; set; }
        public string PaymentMode { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string VerifiedBy { get; set; }
        public string VerifiedRemarks { get; set; }
        public string Status { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class DnbFeeSubmissionVM
    {
        public int FeeStructureId { get; set; }

        public string PaymentReferenceNo { get; set; }

        public string PaymentMode { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Remarks { get; set; }

        public HttpPostedFileBase PaymentScreenshot { get; set; }
    }
}
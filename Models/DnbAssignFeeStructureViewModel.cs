using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class DnbAssignFeeStructureViewModel
    {
        public class YearlyFeeVM
        {
            public int YearNumber { get; set; }
            [Required]
            public decimal Amount { get; set; }
            [Required]
            public string PayableTo { get; set; }
            [Required]
            public DateTime DueDate { get; set; }
        }
        public class AssignFeeStructureViewModel
        {
            public int StudentId { get; set; }
            public string StudentName { get; set; }

            [Required(ErrorMessage = "Enter course duration in years")]
            [Range(1, 10, ErrorMessage = "Duration must be between 1 and 10 years")]
            public int DurationInYears { get; set; }
            public DateTime StartDate { get; set; }
            public List<YearlyFeeVM> YearlyFees { get; set; }

            public AssignFeeStructureViewModel()
            {
                YearlyFees = new List<YearlyFeeVM>();
            }
        }
    }
}
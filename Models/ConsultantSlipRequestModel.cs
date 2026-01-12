using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class ConsultantSlipRequestModel
    {

        public int fk_empid { get; set; }
        public string selectionType { get; set; }  // "Quarter" or "MonthRange"

        [Required(ErrorMessage = "Quarter selection is required.")]
        public int? fk_quarterid { get; set; }  // Quarter ID, nullable to handle unselected value

        [Required(ErrorMessage = "From Month selection is required.")]
        public string fromMonth { get; set; }  // From Month selection

        [Required(ErrorMessage = "To Month selection is required.")]
        public string toMonth { get; set; }  // To Month selection
    }
}
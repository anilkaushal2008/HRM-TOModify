using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class RecieveTransferViewModel
    {
        public int empid { get; set; }
        public int intid { get; set; }
        public string vchTransferFromBranch { get; set; }
        public string vchTransferToBranch { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:dd/MM/yyyy}"))]
        public System.DateTime? dtTransferredDOJ { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:dd/MM/yyyy}"))]
        public System.DateTime? dtRelieved { get; set; }

        [Required(ErrorMessage ="Select recieved date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = ("{0:dd/MM/yyyy}"))]
        public System.DateTime? dtRecieveddoj { get; set; }

        public string vchTransferRemarks { get; set; }
        public int? intTransferSalary { get; set; }

        [Required(ErrorMessage ="Enter salary")]
        [Range(1, int.MaxValue, ErrorMessage = "Recieved salary must be greater than zero.")]
        public int intRecievedSalary { get; set; }

        [Required(ErrorMessage ="Select department")]
        public int fk_ReacievedDept { get; set; }
        public int fk_ReacievedDesi { get; set; }

        [Required(ErrorMessage = "Select position")]
        public int fkPosisionId { get; set; }

        [Required(ErrorMessage ="Enter remarks")]
        public string vchRecievedRemarks { get; set; }
        public Boolean bitIsCorporateemp { get; set; }

    


    }
}
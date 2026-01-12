namespace HRM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tblAssesmentQuestDetail
    {
        [Key]
        public int intid { get; set; }

        public int fk_userid { get; set; }

        public int fk_intEmpAssId { get; set; }

        public int fk_qid { get; set; }

        [Required]
        [StringLength(50)]
        public string vchAnswer { get; set; }

        [StringLength(500)]
        public string vchSpecialRemarks { get; set; }

        public int? intTotal { get; set; }

        [StringLength(50)]
        public string vchAssesmentBy { get; set; }

        public DateTime? dtAssesment { get; set; }

        [StringLength(50)]
        public string vchAssesmentHost { get; set; }

        [StringLength(50)]
        public string vchAssesmentIpused { get; set; }

        [StringLength(50)]
        public string vchAssUpdatedBy { get; set; }

        public DateTime? dtAssUpdated { get; set; }

        [StringLength(50)]
        public string vchAssUpdatedHost { get; set; }

        [StringLength(50)]
        public string vchAssUpdatedIpused { get; set; }

        public bool? BitIsSelected { get; set; }

        public bool? BitIsCompleated { get; set; }

        public int? intcode { get; set; }

        public int? intyr { get; set; }      

        public virtual tblUserMaster tblUserMaster { get; set; }
    }
}

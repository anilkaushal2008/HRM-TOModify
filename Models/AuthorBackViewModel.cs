using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HRM.Models
{
    public class AuthorBackViewModel
    {
        public int id { get; set; }
        [Required(ErrorMessage ="Please select Yes/No")]
        public string bitAuthorBack { get; set; }

        [StringLength(2000, ErrorMessage = "Character length should be equal to or less than 2000 characters")]
        [Required(ErrorMessage ="Enter re-consideration message")]
        public string vchReconsiderRemarks { get; set; }

    }
}
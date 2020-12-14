using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAW_social_platform.Models
{
    public class Profile
    {
        [Key]
        public int ProfileId { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Data nasterii trebuie sa fie de forma dd/mm/yyyy")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public string Hobbies { get; set; }
        [Required(ErrorMessage = "Selectati vizibilitatea profilului!")]
        public bool Status { get; set; }    //  true == public and false == private
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
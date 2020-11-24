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
        public int profileId { get; set; }
        public string description { get; set; }
        [Required(ErrorMessage = "Data nasterii trebuie sa fie de forma dd/mm/yyyy")]
        public DateTime birthDate { get; set; }
        public string hobbies { get; set; }
        public string profileStatus { get; set; }
    }
}
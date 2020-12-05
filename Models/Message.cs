using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAW_social_platform.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        [Required(ErrorMessage = "Mesajul trebuie sa aiba continut!")]
        public string MessageContent { get; set; }
        public DateTime Date { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }

        public virtual Group Group { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
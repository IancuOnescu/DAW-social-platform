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
        public int messageId { get; set; }
        [Required(ErrorMessage = "Mesajul trebuie sa aiba continut!")]
        public string messageContent { get; set; }
        public DateTime date { get; set; }
        public int groupId { get; set; }
        public int userId { get; set; }

        public virtual Group group { get; set; }
        public virtual ApplicationUser user { get; set; }
    }
}
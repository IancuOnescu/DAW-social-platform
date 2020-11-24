using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAW_social_platform.Models
{
    public class Group
    {
        [Key]
        public int groupId { get; set; }
        [Required(ErrorMessage = "Numele grupului este obligatoriu")]
        public string groupName { get; set; }
        public string description { get; set; }

        public virtual ICollection<Message> messages { get; set; }
    }
}
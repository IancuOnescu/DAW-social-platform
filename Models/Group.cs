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
        public int GroupId { get; set; }
        [Required(ErrorMessage = "Numele grupului este obligatoriu")]
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; } 

        public virtual ICollection<Message> Messages { get; set; } 
        public virtual ICollection<GroupUsers> Users { get; set; } 
    }
}
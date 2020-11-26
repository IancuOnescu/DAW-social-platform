using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAW_social_platform.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [Required(ErrorMessage = "Postarea trebuie sa aiba continut")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
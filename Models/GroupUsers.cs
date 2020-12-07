using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DAW_social_platform.Models
{
    public class GroupUsers
    {   
        [Key, Column(Order = 1)]
        public int GroupId { get; set; }
        [Key, Column(Order = 2)]
        public string UserId { get; set; }
        public int RoleId { get; set; }

        public virtual Group Group { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual GroupRoles Role { get; set; }
    }
}
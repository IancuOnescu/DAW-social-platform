using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAW_social_platform.Models
{
    public class GroupRoles
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
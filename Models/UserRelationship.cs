using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DAW_social_platform.Models
{
    public class UserRelationship
    {
        [Key, Column(Order = 1)]
        public string ToUserId { get; set; }
        [Key, Column(Order = 2)]
        public string FromUserId { get; set; }
        public string RelationshipType { get; set; }
    }
}
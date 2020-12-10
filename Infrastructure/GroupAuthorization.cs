using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAW_social_platform.Models;

namespace DAW_social_platform.Infrastructure
{
    public class GroupAuthorization
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserOrAdminOrCreator(int groupId, string userId)
        {
            var userRole = (from gu in db.GroupUsers
                            where gu.UserId == userId && gu.GroupId == groupId
                            select new
                            { gu.Role.RoleName }).FirstOrDefault();
            if (userRole is null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsAdminOrCreator(int groupId, string userId)
        {
            var userRole = (from gu in db.GroupUsers
                            where gu.UserId == userId && gu.GroupId == groupId
                            select new
                            { gu.Role.RoleName }).FirstOrDefault();
            if (userRole is null ||  userRole.RoleName == "User")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsCreator(int groupId, string userId)
        {
            var userRole = (from gu in db.GroupUsers
                            where gu.UserId == userId && gu.GroupId == groupId
                            select new
                            { gu.Role.RoleName }).FirstOrDefault();
            if (userRole is null || userRole.RoleName == "User" || userRole.RoleName == "Admin")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
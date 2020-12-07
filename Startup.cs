using DAW_social_platform.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DAW_social_platform.Startup))]
namespace DAW_social_platform
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateAdminUserAndApplicationRoles();
            CreateGroupRoles();
        }
        private void CreateAdminUserAndApplicationRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new
            RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new
            UserStore<ApplicationUser>(context));
            // Se adauga rolurile aplicatiei
            if (!roleManager.RoleExists("Admin"))
            {
                // Se adauga rolul de administrator
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);
                // se adauga utilizatorul administrator
                var user = new ApplicationUser();
                user.UserName = "admin@gmail.com";
                user.Email = "admin@gmail.com";
                var adminCreated = UserManager.Create(user, "!1Admin");
                if (adminCreated.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Admin");
                }
            }
            if (!roleManager.RoleExists("Editor"))
            {
                var role = new IdentityRole();
                role.Name = "Editor";
                roleManager.Create(role);
            }
            if (!roleManager.RoleExists("User"))
            {
                var role = new IdentityRole();
                role.Name = "User";
                roleManager.Create(role);
            }
        }
        public void CreateGroupRoles()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var roles = db.GroupRoles;

            var creatorFound = false;
            var adminFound = false;
            var userFound = false;
            foreach (GroupRoles role in roles)
            {
                if (role.RoleName == "Creator")
                {
                    creatorFound = true;
                }
                if (role.RoleName == "Admin")
                {
                    adminFound = true;
                }
                if (role.RoleName == "User")
                {
                    userFound = true;
                }
            }
            if (!creatorFound)
            {
                var newRole = new GroupRoles();
                newRole.RoleName = "Creator";
                db.GroupRoles.Add(newRole);
                db.SaveChanges();
            }
            if (!adminFound)
            { 
                var newRole = new GroupRoles();
                newRole.RoleName = "Admin";
                db.GroupRoles.Add(newRole);
                db.SaveChanges();
            }
            if (!userFound)
            {
                var newRole = new GroupRoles();
                newRole.RoleName = "User";
                db.GroupRoles.Add(newRole);
                db.SaveChanges();
            }
        }
    }
}

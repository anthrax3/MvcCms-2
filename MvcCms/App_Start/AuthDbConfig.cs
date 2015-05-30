using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MvcCms.Data;
using MvcCms.Models;

// ReSharper disable once CheckNamespace
namespace MvcCms.App_Start
{
    public class AuthDbConfig
    {
        public static void RegisterAdmin()
        {
            using(var users = new UserRepository())
            {
                var user = users.GetUserByName("admin");
                if(user == null)
                {
                    var adminUser = new CmsUser
                    {
                        UserName = "admin",
                        Email = "admin@cms.com",
                        DisplayName = "Administrator"
                    };
                    users.Create(adminUser, "Password1234");
                }
            }

            using (var roles = new RoleRepository())
            {
                if (roles.GetRoleByName("admin") == null)
                {
                    roles.Create(new IdentityRole("admin"));
                }

                if (roles.GetRoleByName("editor") == null)
                {
                    roles.Create(new IdentityRole("editor"));
                }

                if (roles.GetRoleByName("author") == null)
                {
                    roles.Create(new IdentityRole("author"));
                }
            }
        }
    }
}

using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MvcCms.Data
{
    public interface IRoleRepository
    {
        IdentityRole GetRoleByName(string name);
        IEnumerable<IdentityRole> GetAllRoles();
        void Create(IdentityRole role);
    }
}
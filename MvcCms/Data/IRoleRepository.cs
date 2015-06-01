using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MvcCms.Data
{
    public interface IRoleRepository
    {
        Task<IdentityRole> GetRoleByNameAsync(string name);
        Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
        Task CreateAsync(IdentityRole role);
    }
}
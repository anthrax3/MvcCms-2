using System.Collections.Generic;
using MvcCms.Models;

namespace MvcCms.Data
{
    public interface IUserRepository
    {
        CmsUser GetUserByName(string username);
        IEnumerable<CmsUser> GetAllUsers();
        void Create(CmsUser user, string password);
        void Delete(CmsUser user);
    }
}
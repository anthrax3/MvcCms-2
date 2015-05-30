using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcCms.Models;

namespace MvcCms.Data
{
    public class UserRepository : IUserRepository, IDisposable
    {
        private readonly CmsUserStore _store;
        private readonly CmsUserManager _manager;

        public UserRepository()
        {
            _store = new CmsUserStore();
            _manager = new CmsUserManager();
        }

        public CmsUser GetUserByName(string username)
        {
            return _store.FindByNameAsync(username).Result;
        }

        public IEnumerable<CmsUser> GetAllUsers()
        {
            return _store.Users.ToArray();
        }

        public void Create(CmsUser user, string password)
        {
            var result = _manager.CreateAsync(user, password).Result;
        }

        public void Delete(CmsUser user)
        {
            var result = _manager.DeleteAsync(user).Result;
        }

        public void Update(CmsUser user)
        {
            var result = _manager.UpdateAsync(user).Result;
        }

        private bool _disposed = false;

        public void Dispose()
        {
            if(_disposed)
            {
                _store.Dispose();
                _manager.Dispose();
            }

            _disposed = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
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

        public async Task<CmsUser> GetUserByNameAsync(string username)
        {
            return await _store.FindByNameAsync(username);
        }

        public IEnumerable<CmsUser> GetAllUsers()
        {
            return _store.Users.ToArray();
        }

        public async Task CreateAsync(CmsUser user, string password)
        {
            await _manager.CreateAsync(user, password);            
        }

        public async Task DeleteAsync(CmsUser user)
        {
            await _manager.DeleteAsync(user);
        }

        public async Task UpdateAsync(CmsUser user)
        {
            await _manager.UpdateAsync(user);            
        }

        public string HashPassword(string newPassword)
        {
            return _manager.PasswordHasher.HashPassword(newPassword);
        }

        public bool VerifyPassword(string passwordHash, string currentPassword)
        {
            return _manager.PasswordHasher.VerifyHashedPassword(passwordHash, currentPassword)
                    == PasswordVerificationResult.Success;
        }

        private bool _disposed;

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

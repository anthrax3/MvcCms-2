using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
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
            _manager = new CmsUserManager(_store);
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

        public async Task AddUserToRoleAsync(CmsUser user, string role)
        {
            await _manager.AddToRoleAsync(user.Id, role);
        }

        public async Task<IEnumerable<string>> GetRolesForUserAsync(CmsUser user)
        {
            return await _manager.GetRolesAsync(user.Id);
        }

        public async Task RemoveUserFromRoleAsync(CmsUser user, params string[] roleNames)
        {
            await _manager.RemoveFromRolesAsync(user.Id, roleNames);
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

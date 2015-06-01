using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using MvcCms.Areas.Admin.ViewModels;
using MvcCms.Data;
using MvcCms.Models;

namespace MvcCms.Areas.Admin.Services
{
    public class UserService
    {
        private readonly IUserRepository _users;
        private readonly IRoleRepository _roles;
        private readonly ModelStateDictionary _modelState;

        public UserService(ModelStateDictionary modelState, IUserRepository userRepository,
            IRoleRepository roleRepository)
        {
            _modelState = modelState;
            _users = userRepository;
            _roles = roleRepository;
        }

        public async Task<UserViewModel> FindByNameAsync(string username)
        {
            var user = await _users.GetUserByNameAsync(username);

            if (user == null)
            {
                throw new KeyNotFoundException("The user does not exist");
            }

            var viewModel = new UserViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                DisplayName = user.DisplayName
            };

            var userRoles = await _users.GetRolesForUserAsync(user);
            var enumerable = userRoles as string[] ?? userRoles.ToArray();
            viewModel.SelectedRole = enumerable.Count() > 1 ? enumerable.FirstOrDefault() : enumerable.SingleOrDefault();

            viewModel.LoadUserRoles(await _roles.GetAllRolesAsync());

            return viewModel;
        }

        public IEnumerable<CmsUser> GetAllUsers()
        {
            return _users.GetAllUsers();
        }

        public async Task<bool> CreateAsync(UserViewModel model)
        {
            if (!_modelState.IsValid)
            {
                return false;
            }

            var existingUser = await _users.GetUserByNameAsync(model.Username);

            if (existingUser != null)
            {
                _modelState.AddModelError(string.Empty, "The user already exists");
                return false;
            }

            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                _modelState.AddModelError(string.Empty, "You must provide a password");
                return false;
            }

            var newUser = new CmsUser
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Username
            };

            await _users.CreateAsync(newUser, model.NewPassword);

            await _users.AddUserToRoleAsync(newUser, model.SelectedRole);

            return true;
        }

        public async Task<bool> EditAsync(UserViewModel model)
        {
            var user = await _users.GetUserByNameAsync(model.Username);            

            if (user == null)
            {
                throw new KeyNotFoundException("The user does not exist");
            }

            if (!_modelState.IsValid)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(model.CurrentPassword))
                {
                    _modelState.AddModelError(string.Empty, "The current password must be supplied");
                    return false;
                } 

                var passwordVerified = _users.VerifyPassword(user.PasswordHash, model.CurrentPassword);

                if (!passwordVerified)
                {
                    _modelState.AddModelError(string.Empty, "The current password does not match");
                    return false;
                }

                var newHashedPassword = _users.HashPassword(model.NewPassword);
                user.PasswordHash = newHashedPassword;
            }

            user.Email = model.Email;
            user.DisplayName = model.DisplayName;

            await _users.UpdateAsync(user);

            var roles = await _users.GetRolesForUserAsync(user);
            await _users.RemoveUserFromRoleAsync(user, roles.ToArray());

            await _users.AddUserToRoleAsync(user, model.SelectedRole);

            return true;
        }

        public async Task DeleteAsync(string username)
        {

            var user = await _users.GetUserByNameAsync(username);

            if (user == null)
            {
                throw new KeyNotFoundException("The user does not exist");
            }

            await _users.DeleteAsync(user);

        }
    }
}

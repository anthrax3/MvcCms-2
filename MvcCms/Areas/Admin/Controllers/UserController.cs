using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MvcCms.Areas.Admin.ViewModels;
using MvcCms.Data;

namespace MvcCms.Areas.Admin.Controllers
{
    [RouteArea("admin")]
    [RoutePrefix("user")]
    public class UserController : Controller
    {
        [Route("")]
        public ActionResult Index()
        {
            using(var manager = new CmsUserManager())
            {
                var users = manager.Users.ToList();
                return View(users);
            }            
        }
        
        [Route("edit/{username}")]
        [HttpGet]
        public ActionResult Edit(string username)
        {
            using(var userStore = new CmsUserStore())
            using(var userManager = new CmsUserManager(userStore))
            {
                var user = userStore.FindByNameAsync(username).Result;

                if(user == null)
                {
                    return HttpNotFound();
                }

                var viewModel = new UserViewModel
                {
                    Username = user.UserName,
                    Email = user.Email
                };

                return View(viewModel);
            }
        }

        [Route("edit/{username}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserViewModel model)
        {
            using (var userStore = new CmsUserStore())
            using (var userManager = new CmsUserManager(userStore))
            {
                var user = userStore.FindByNameAsync(model.Username).Result;

                if (user == null)
                {
                    return HttpNotFound();
                }

                if(!ModelState.IsValid)
                {
                    return View(model);
                }

                if(!string.IsNullOrWhiteSpace(model.NewPassword))
                {
                    if(string.IsNullOrWhiteSpace(model.CurrentPassword))
                    {
                        ModelState.AddModelError(string.Empty, "The current password must be supplied");
                        return View(model);
                    }

                    var passwordVerified = userManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash,
                        model.CurrentPassword);

                    if(passwordVerified != PasswordVerificationResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The current password does not match");
                        return View(model);
                    }

                    var newHashedPassword = userManager.PasswordHasher.HashPassword(model.NewPassword);
                    user.PasswordHash = newHashedPassword;
                }

                user.Email = model.Email;
                user.DisplayName = model.DisplayName;

                var updateResult = userManager.UpdateAsync(user).Result;
                if(updateResult.Succeeded)
                {
                    return RedirectToAction("index");
                }

                ModelState.AddModelError(string.Empty, "An error occurred. Please try again.");
                return View(model);
            }
        }

        [Route("delete/{username}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string username)
        {
            using (var userStore = new CmsUserStore())
            using (var userManager = new CmsUserManager(userStore))
            {
                var user = userStore.FindByNameAsync(username).Result;

                if (user == null)
                {
                    return HttpNotFound();
                }

                var deleteResult = userManager.DeleteAsync(user).Result;

                return RedirectToAction("index");
            }
        }
    }
}

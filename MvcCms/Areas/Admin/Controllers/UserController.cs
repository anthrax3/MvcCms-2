using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MvcCms.Areas.Admin.Services;
using MvcCms.Areas.Admin.ViewModels;
using MvcCms.Data;

namespace MvcCms.Areas.Admin.Controllers
{
    [RouteArea("admin")]
    [RoutePrefix("user")]
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly RoleRepository _roleRepository;
        private readonly UserService _users;

        public UserController()
        {
            _userRepository = new UserRepository();
            _roleRepository = new RoleRepository();
            _users = new UserService(ModelState, _userRepository, _roleRepository);
        }

        [Route("")]
        public ActionResult Index()
        {
            var users = _users.GetAllUsers();
            return View(users);
        }

        [Route("create")]
        [HttpGet]
        public ActionResult Create()
        {
            var model = new UserViewModel();
            return View(model);
        }

        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserViewModel model)
        {
            if (await _users.CreateAsync(model))
            {
                return RedirectToAction("index");
            }

            return View(model);
        }

        [Route("edit/{username}")]
        [HttpGet]
        public async Task<ActionResult> Edit(string username)
        {
            try
            {
                var user = await _users.FindByNameAsync(username);

                return View(user);
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }

        }

        [Route("edit/{username}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserViewModel model)
        {
            try
            {
                if (await _users.EditAsync(model))
                {
                    return RedirectToAction("index");
                }
                
                return View(model);
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }
        }

        [Route("delete/{username}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string username)
        {
            try
            {
                await _users.DeleteAsync(username);
                return RedirectToAction("index");
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }
        }

        private bool _isDisposed;

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                _userRepository.Dispose();
                _roleRepository.Dispose();
            }

            _isDisposed = true;
            base.Dispose(disposing);
        }
    }
}

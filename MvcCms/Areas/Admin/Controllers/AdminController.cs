using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using MvcCms.Areas.Admin.ViewModels;
using MvcCms.Data;

namespace MvcCms.Areas.Admin.Controllers
{
    [RouteArea("admin")]
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IUserRepository _users;

        public AdminController(IUserRepository userRepository)
        {
            _users = userRepository;
        }

        public AdminController() : this(new UserRepository())
        {            
        }

        // GET: Admin/Admin
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("login")]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            var user = await _users.GetLoginUserAsync(model.Username, model.Password);
            if(user == null)
            {
                ModelState.AddModelError(string.Empty, "The user with the supplied credentials does not exist.");
                return View();
            }

            var authManager = HttpContext.GetOwinContext().Authentication;
            var userIdentity = await _users.CreateIdentityAsync(user);
            authManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = model.RememberMe                
            }, userIdentity);

            return RedirectToAction("index");
        }

        [Route("logout")]
        public ActionResult Logout()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();

            return RedirectToAction("index", "home");
        }
    }
}
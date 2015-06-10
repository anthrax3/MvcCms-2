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
            if(User.Identity.IsAuthenticated)
            {
                return RedirectToAction("index");
            }

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

        [AllowAnonymous]
        public PartialViewResult AdminMenu()
        {
            var items = new List<AdminMenuItem>();
            
            if(!User.Identity.IsAuthenticated)
            {
                return PartialView(items);
            }

            items.Add(new AdminMenuItem
            {
                Text = "Admin Home",
                Action = "index",
                RouteInfo = new { controller = "admin", area = "admin"}
            });

            if(User.IsInRole("admin"))
            {
                items.Add(new AdminMenuItem
                {
                    Text = "Users",
                    Action = "index",
                    RouteInfo = new {controller = "user", area = "admin"}
                });
            }
            else
            {
                items.Add(new AdminMenuItem
                {
                    Text = "Profile",
                    Action = "edit",
                    RouteInfo = new { controller = "user", area = "admin", username = User.Identity.Name }
                });
            }

            if(!User.IsInRole("author"))
            {
                items.Add(new AdminMenuItem
                {
                    Text = "Tags",
                    Action = "index",
                    RouteInfo = new { controller = "tag", area = "admin" }
                });
            }

            items.Add(new AdminMenuItem
            {
                Text = "Posts",
                Action = "index",
                RouteInfo = new { controller = "post", area = "admin" }
            });

            return PartialView(items);
        }

        [AllowAnonymous]
        public PartialViewResult AuthenticationLink()
        {
            var item = new AdminMenuItem
            {
                RouteInfo = new { controller = "admin", area = "admin" }
            };

            if(User.Identity.IsAuthenticated)
            {
                item.Text = "Logout";
                item.Action = "logout";
            }
            else
            {
                item.Text = "Login";
                item.Action = "login";
            }

            return PartialView("_menuLink", item);
        }

        private bool _isDisposed;

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                _users.Dispose();                
            }

            _isDisposed = true;
            base.Dispose(disposing);
        }
    }
}
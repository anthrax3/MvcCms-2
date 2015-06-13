using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MvcCms.Data;
using MvcCms.Models;

namespace MvcCms.Controllers
{
    [RoutePrefix("")]
    public class HomeController : Controller
    {
        private IPostRepository _posts;
        private readonly int _pageSize = 2;

        public HomeController() : this(new PostRepository())
        {            
        }

        public HomeController(IPostRepository postRepository)
        {
            _posts = postRepository;
        }

        // GET: Default
        [Route("")]
        public async Task<ActionResult> Index()
        {
            var posts = await _posts.GetPageAsync(1, _pageSize);
            ViewBag.PreviousPage = 0;
            ViewBag.NextPage = (Decimal.Divide(_posts.CountPublished, _pageSize) > 1) ? 2 : -1;

            return View(posts);
        }

        public async Task<ActionResult> Page(int page = 1)
        {
            if(page < 2)
            {
                return RedirectToAction("index");
            }

            var posts = await _posts.GetPageAsync(page, _pageSize);
            ViewBag.PreviousPage = page - 1;
            ViewBag.NextPage = (Decimal.Divide(_posts.CountPublished, _pageSize) > page) ? page + 1 : -1;

            return View("index", posts);
        }

        [Route("posts/{postId}")]
        public async Task<ActionResult> Post(string postId)
        {
            var post = await _posts.GetAsync(postId);

            if(post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        [Route("tags/{tagId}")]
        public async Task<ActionResult> Tag(string tagId)
        {
            var posts = await _posts.GetPostsByTagAsync(tagId);

            if(!posts.Any())
            {
                return HttpNotFound();
            }

            ViewBag.Tag = tagId;

            return View(posts);
        } 
    }
}
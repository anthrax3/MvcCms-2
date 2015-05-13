using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCms.Data;
using MvcCms.Models;

namespace MvcCms.Areas.Admin.Controllers
{
    // /admin/post
    [RouteArea("Admin")]
    public class PostController : Controller
    {
        private readonly IPostRepository _repository;

        public PostController(IPostRepository repository)
        {
            _repository = repository;
        }

        // GET: Admin/Post
        public ActionResult Index()
        {
            var posts = _repository.GetAll();
            return View(posts);
        }

        // /admin/post/create
        [HttpGet]
        [Route("create")]
        public ActionResult Create()
        {
            var model = new Post();

            return View(model);
        }

        // /admin/post/create
        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Post model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _repository.Create(model);

            return View(model);
        }

        // /admin/post/edit/post-to-edit
        [HttpGet]
        [Route("edit/{postId}")]
        public ActionResult Edit(string postId)
        {
           // TODO: to retrieve the model from the data store            
            var post = _repository.Get(postId);

            if(post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // /admin/post/edit/post-to-edit
        [HttpPost]
        [Route("edit/{postId}")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string postId, Post model)
        {
            var post = _repository.Get(postId);
            if(post == null)
            {
                return HttpNotFound();
            }
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            _repository.Edit(postId, model);

            // TODO: update data model in data store

            return RedirectToAction("index");
        }
    }
}
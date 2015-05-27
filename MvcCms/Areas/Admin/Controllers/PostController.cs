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
    [RoutePrefix("post")]
    public class PostController : Controller
    {
        private readonly IPostRepository _repository;

        public PostController() : 
            this(new PostRepository())
        {
            
        }

        public PostController(IPostRepository repository)
        {
            _repository = repository;
        }

        // GET: Admin/Post
        [Route("")]
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
            return View(new Post());
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

            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = model.Title;
            }

            model.Id = model.Id.MakeUrlFriendly();
            model.Tags = model.Tags.Select(t => t.MakeUrlFriendly()).ToList();
            model.Created = DateTime.Now;
            model.AuthorId = "e31095c2-382f-4a8a-975d-33e809b2da84";

            try
            {
                _repository.Create(model);

                return RedirectToAction("index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("key", e);
                return View(model);
            }
        }

        // /admin/post/edit/post-to-edit
        [HttpGet]
        [Route("edit/{postId}")]
        public ActionResult Edit(string postId)
        {                      
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
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = model.Title;
            }

            model.Id = model.Id.MakeUrlFriendly();
            model.Tags = model.Tags.Select(t => t.MakeUrlFriendly()).ToList();

            try
            {
                _repository.Edit(postId, model);

                return RedirectToAction("index");
            }
            catch(KeyNotFoundException)
            {
                return HttpNotFound();
            }
            catch(Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }
        }

        [HttpGet]
        [Route("delete/{postId}")]
        public ActionResult Delete(string postId)
        {            
            var post = _repository.Get(postId);

            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // /admin/post/delete/post-to-edit
        [HttpPost]
        [Route("delete/{postId}")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string postId, string dummy)
        {            
            try
            {
                _repository.Delete(postId);

                return RedirectToAction("index");
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }            
        }
    }
}
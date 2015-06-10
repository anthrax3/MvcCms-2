using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCms.Data;

namespace MvcCms.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("tag")]
    [Authorize(Roles = "admin, editor")]
    public class TagController : Controller
    {
        private readonly ITagRepository _repository;

        public TagController() : this(new TagRepository())
        {            
        }

        public TagController(ITagRepository repository)
        {
            _repository = repository;
        }

        // GET: Admin/Tag
        [Route("")]
        public ActionResult Index()
        {
            var tags = _repository.GetAll();

            return View(tags);
        }

        [HttpGet]
        [Route("edit/{tag}")]
        public ActionResult Edit(string tag)
        {
            try
            {
                var model = _repository.Get(tag);
                return View(model: model);
            }
            catch(KeyNotFoundException)
            {
                return HttpNotFound();
            }                                 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("edit/{tag}")]
        public ActionResult Edit(string tag, string newTag)
        {
            var tags = _repository.GetAll();
            var enumerable = tags as string[] ?? tags.ToArray();

            if (!enumerable.Contains(tag))
            {
                return HttpNotFound();
            }

            if(enumerable.Contains(newTag))
            {
                return RedirectToAction("index");
            }

            if(string.IsNullOrWhiteSpace(newTag))
            {
                ModelState.AddModelError("key", "New tag value cannot ve empty");
                
                return View(model: tag);
            }

            _repository.Edit(tag, newTag);

            return RedirectToAction("index");
        }

        [HttpGet]
        [Route("delete/{tag}")]
        public ActionResult Delete(string tag)
        {
            try
            {
                var model = _repository.Get(tag);
                return View(model: model);
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            } 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("delete/{tag}")]
        public ActionResult Delete(string tag, string foo)
        {
            try
            {
                _repository.Delete(tag);

                return RedirectToAction("index");
            }
            catch (KeyNotFoundException)
            {
                return HttpNotFound();
            }
        }
    }
}
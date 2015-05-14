using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcCms.Data;

namespace MvcCms.Areas.Admin.Controllers
{
    public class TagController : Controller
    {
        private readonly ITagRepository _repository;

        public TagController(ITagRepository repository)
        {
            _repository = repository;
        }

        // GET: Admin/Tag
        public ActionResult Index()
        {
            var tags = _repository.GetAll();

            return View(tags);
        }

        [HttpGet]
        public ActionResult Edit(string tag)
        {
            if(!_repository.Exists(tag))
            {
                return HttpNotFound();
            }

            return View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                
                return View(tag);
            }

            _repository.Edit(tag, newTag);

            return RedirectToAction("index");
        }

        [HttpGet]
        public ActionResult Delete(string tag)
        {
            if (!_repository.Exists(tag))
            {
                return HttpNotFound();
            }

            return View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string tag, string dummy)
        {              
            if (!_repository.Exists(tag))
            {
                return HttpNotFound();
            }                        

            _repository.Delete(tag);

            return RedirectToAction("index");
        }
    }
}
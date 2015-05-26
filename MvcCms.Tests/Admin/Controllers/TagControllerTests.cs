using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcCms.Areas.Admin.Controllers;
using MvcCms.Data;
using Telerik.JustMock;

namespace MvcCms.Tests.Admin.Controllers
{
    [TestClass]
    public class TagControllerTests
    {            
        [TestMethod]
        public void Edit_GetRequestSendsTagToView()
        {
            var tag = "my tag";
            var repo = Mock.Create<ITagRepository>();
            var controller = new TagController(repo);

            Mock.Arrange(() => repo.Get(tag))
                .Returns(tag.ToLower());

            var result = (ViewResult)controller.Edit(tag);
            var model = (string) result.Model;

            Assert.AreEqual(tag, model);
        }

        [TestMethod]
        public void Edit_GetRequestNotFoundResult()
        {
            var tag = "my tag";
            var repo = Mock.Create<ITagRepository>();
            var controller = new TagController(repo);
            Mock.Arrange(() => repo.Get(tag))
                .Throws(new KeyNotFoundException());

            var result = controller.Edit(tag);            

            Assert.IsTrue(result is HttpNotFoundResult);
        }

        [TestMethod]
        public void Edit_PostRequestNotFoundResult()
        {
            var tag = "my tag";
            var newTag = "new tag";
            var repo = Mock.Create<ITagRepository>();
            var controller = new TagController(repo);
            
            Mock.Arrange(() => repo.GetAll())
                .Returns(new[]{"tag1", "tag2"});

            var result = controller.Edit(tag, newTag);

            Assert.IsTrue(result is HttpNotFoundResult);
        }

        [TestMethod]
        public void Edit_PostRequestContainsNewTag()
        {
            var tag = "my tag";
            var newTag = "new tag";
            var repo = Mock.Create<ITagRepository>();
            var controller = new TagController(repo);
            
            Mock.Arrange(() => repo.GetAll())
                .Returns(new[] { "tag1", "my tag", "new tag" });

            Mock.Arrange(() => repo.Edit(Arg.IsAny<string>(), Arg.IsAny<string>()))
                .OccursNever();

            var result = controller.Edit(tag, newTag);

            Mock.Assert(repo);

            Assert.IsTrue(result is RedirectToRouteResult);
        }

        [TestMethod]
        public void Edit_PostRequestErrorModelState()
        {
            var tag = "my tag";
            var newTag = "";
            var repo = Mock.Create<ITagRepository>();
            var controller = new TagController(repo);

            Mock.Arrange(() => repo.GetAll())
                .Returns(new[] { "tag1", "my tag", "new tag" });
            
            var result = (ViewResult) controller.Edit(tag, newTag);

            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.AreEqual(tag, result.Model);
        }
    }
}

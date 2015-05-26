using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcCms.Areas.Admin.Controllers;
using MvcCms.Data;
using MvcCms.Models;
using Telerik.JustMock;

namespace MvcCms.Tests.Admin.Controllers
{
    [TestClass]
    public class PostControllerTests
    {        

        [TestMethod]
        public void Edit_GetRequestSendsPostToView()
        {
            var id = "test-post";
            var repo = Mock.Create<IPostRepository>();
            var controller = new PostController(repo);

            Mock.Arrange(() => repo.Get(id))
                .Returns(new Post {Id = id});

            var result = (ViewResult) controller.Edit(id);
            var model = (Post) result.Model;

            Assert.AreEqual(id, model.Id);
        }

        [TestMethod]
        public void Edit_GetRequestNotFoundResult()
        {
            var id = "test-post";
            var repo = Mock.Create<IPostRepository>();
            var controller = new PostController(repo);

            Mock.Arrange(() => repo.Get(id))
                .Returns((Post)null);

            var result = controller.Edit(id);            

            Assert.IsTrue(result is HttpNotFoundResult);
        }

        [TestMethod]
        public void Edit_PostRequestNotFoundResult()
        {
            var id = "test-post";
            var repo = Mock.Create<IPostRepository>();
            var controller = new PostController(repo);
            var post = new Post {Id = "Mock Post"};
            Mock.Arrange(() => repo.Edit(id, post))
                .Throws(new KeyNotFoundException());            

            var result = controller.Edit(id, post);

            Assert.IsTrue(result is HttpNotFoundResult);
        }

        [TestMethod]
        public void Edit_PostRequestSendsPostToView()
        {
            var id = "test-post";
            var repo = Mock.Create<IPostRepository>();
            var controller = new PostController(repo);

            Mock.Arrange(() => repo.Get(id))
                .Returns(new Post { Id = id });

            controller.ViewData.ModelState.AddModelError("key", "error message");

            var result = (ViewResult)controller.Edit(id, new Post(){Id = "test-post-2"});
            var model = (Post)result.Model;

            Assert.AreEqual("test-post-2", model.Id);
        }

        [TestMethod]
        public void Edit_PostRequestCallsEditAndRedirects()
        {            
            var repo = Mock.Create<IPostRepository>();
            var controller = new PostController(repo);

            Mock.Arrange(() => repo.Edit(Arg.IsAny<string>(), Arg.IsAny<Post>()))
                .MustBeCalled();

            var result = controller.Edit("foo", new Post { Id = "test-post-2" });

            Mock.Assert(repo);
            Assert.IsTrue(result is RedirectToRouteResult);
        }        

        [TestMethod]
        public void Create_PostRequestErrorModelState()
        {            
            var repo = Mock.Create<IPostRepository>();
            var controller = new PostController(repo);
            controller.ViewData.ModelState.AddModelError("key", "error message");

            var result = (ViewResult)controller.Create(new Post{Id = "test-id"});
            var model = (Post)result.Model;

            Assert.AreEqual("test-id", model.Id);
        }

        [TestMethod]
        public void Create_PostRequestCallsCreateSendPostToView()
        {
            var repo = Mock.Create<IPostRepository>();
            var controller = new PostController(repo);

            Mock.Arrange(() => repo.Create(Arg.IsAny<Post>()))
                .MustBeCalled();

            var result = controller.Create(new Post { Id = "test-id" });

            Mock.Assert(repo);
            Assert.IsTrue(result is RedirectToRouteResult);
        }

        [TestMethod]
        public void Create_GetRequestSendsModelToView()
        {            
            var repo = Mock.Create<IPostRepository>();
            var controller = new PostController(repo);            

            var result = (ViewResult)controller.Create();
            var model = result.Model;

            Assert.IsTrue(model is Post);
        }
    }
}

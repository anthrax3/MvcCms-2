using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcCms.Areas.Admin.Controllers;
using MvcCms.Data;
using MvcCms.Models;
using Telerik.JustMock;
using System.Threading.Tasks;

namespace MvcCms.Tests.Admin.Controllers
{
    [TestClass]
    public class PostControllerTests
    {        

        [TestMethod]
        public async Task Edit_GetRequestSendsPostToView()
        {
            var id = "test-post";
            var repo = Mock.Create<IPostRepository>();
            var userRepo = Mock.Create<IUserRepository>();
            var controller = new PostController(repo, userRepo);

            Mock.Arrange(() => repo.Get(id))
                .Returns(new Post {Id = id});

            var result = (ViewResult) await controller.Edit(id);
            var model = (Post) result.Model;

            Assert.AreEqual(id, model.Id);
        }

        [TestMethod]
        public async Task Edit_GetRequestNotFoundResult()
        {
            var id = "test-post";
            var repo = Mock.Create<IPostRepository>();
            var userRepo = Mock.Create<IUserRepository>();
            var controller = new PostController(repo, userRepo);

            Mock.Arrange(() => repo.Get(id))
                .Returns((Post)null);

            var result = await controller.Edit(id);            

            Assert.IsTrue(result is HttpNotFoundResult);
        }

        [TestMethod]
        public async Task Edit_PostRequestNotFoundResult()
        {
            var id = "test-post";
            var repo = Mock.Create<IPostRepository>();
            var userRepo = Mock.Create<IUserRepository>();
            var controller = new PostController(repo, userRepo);
            var post = new Post {Id = "Mock Post"};
            Mock.Arrange(() => repo.Edit(id, post))
                .Throws(new KeyNotFoundException());            

            var result = await controller.Edit(id, post);

            Assert.IsTrue(result is HttpNotFoundResult);
        }

        [TestMethod]
        public async Task Edit_PostRequestSendsPostToView()
        {
            var id = "test-post";
            var repo = Mock.Create<IPostRepository>();
            var userRepo = Mock.Create<IUserRepository>();
            var controller = new PostController(repo, userRepo);

            Mock.Arrange(() => repo.Get(id))
                .Returns(new Post { Id = id });

            controller.ViewData.ModelState.AddModelError("key", "error message");

            var result = (ViewResult)await controller.Edit(id, new Post(){Id = "test-post-2"});
            var model = (Post)result.Model;

            Assert.AreEqual("test-post-2", model.Id);
        }

        [TestMethod]
        public async Task Edit_PostRequestCallsEditAndRedirects()
        {            
            var repo = Mock.Create<IPostRepository>();
            var userRepo = Mock.Create<IUserRepository>();
            var controller = new PostController(repo, userRepo);

            Mock.Arrange(() => repo.Edit(Arg.IsAny<string>(), Arg.IsAny<Post>()))
                .MustBeCalled();

            var result = await controller.Edit("foo", new Post { Id = "test-post-2" });

            Mock.Assert(repo);
            Assert.IsTrue(result is RedirectToRouteResult);
        }        

        [TestMethod]
        public async Task Create_PostRequestErrorModelState()
        {            
            var repo = Mock.Create<IPostRepository>();
            var userRepo = Mock.Create<IUserRepository>();
            var controller = new PostController(repo, userRepo);
            controller.ViewData.ModelState.AddModelError("key", "error message");

            var result = (ViewResult)await controller.Create(new Post{Id = "test-id"});
            var model = (Post)result.Model;

            Assert.AreEqual("test-id", model.Id);
        }

        [TestMethod]
        public async Task Create_PostRequestCallsCreateSendPostToView()
        {
            var repo = Mock.Create<IPostRepository>();
            var userRepo = Mock.Create<IUserRepository>();
            var controller = new PostController(repo, userRepo);

            Mock.Arrange(() => repo.Create(Arg.IsAny<Post>()))
                .MustBeCalled();

            var result = await controller.Create(new Post { Id = "test-id" });

            Mock.Assert(repo);
            Assert.IsTrue(result is RedirectToRouteResult);
        }

        [TestMethod]
        public void Create_GetRequestSendsModelToView()
        {            
            var repo = Mock.Create<IPostRepository>();
            var userRepo = Mock.Create<IUserRepository>();
            var controller = new PostController(repo, userRepo);

            var result = (ViewResult)controller.Create();
            var model = result.Model;

            Assert.IsTrue(model is Post);
        }
    }
}

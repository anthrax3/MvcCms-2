using System;
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

            var result = (ViewResult) controller.Edit(id);
            var model = (Post) result.Model;

            Assert.AreEqual(id, model.Id);
        }
    }
}

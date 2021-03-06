﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcCms.App_Start;
using MvcCms.Models;
using MvcCms.Models.ModelBinders;

namespace MvcCms
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {            
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            AreaRegistration.RegisterAllAreas();
            AuthDbConfig.RegisterAdminAsync();
            ModelBinders.Binders.Add(typeof(Post), new PostModelBinder());
        }
    }
}

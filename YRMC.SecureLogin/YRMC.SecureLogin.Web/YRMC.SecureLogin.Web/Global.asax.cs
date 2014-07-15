﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace YRMC.SecureLogin.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.AuthenticationType != "Windows" &&
                    HttpContext.Current.User.Identity.AuthenticationType != "Negotiate" &&
                    HttpContext.Current.User.Identity.AuthenticationType != "NTLM")
                {
                    throw new Exception("Only windows authentication is supported, not " +
                        HttpContext.Current.User.Identity.AuthenticationType);
                }
                else
                {
                    if (Csla.ApplicationContext.ClientContext["Csla.Principal"] == null)
                    {
                        YRMC.SecureLogin.Business.Security.SecurityPrincipal.Login(
                            ((System.Security.Principal.WindowsIdentity)HttpContext.Current.User.Identity).User.Value);

                        Csla.ApplicationContext.ClientContext["Csla.Principal"] = Csla.ApplicationContext.User;
                    }
                    else
                    {
                        Csla.ApplicationContext.User =
                            (System.Security.Principal.IPrincipal)Csla.ApplicationContext.ClientContext["Csla.Principal"];
                    }
                }
            }
        }
    }
}
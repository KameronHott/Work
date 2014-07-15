using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YRMC.SecureLogin.Web.Controllers
{
    [RequireHttps]
    public class RoleController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult List()
        {
            return Json(((Business.Security.SecurityIdentity)Csla.ApplicationContext.User.Identity)
                .Roles.Select(o => new
                {
                    id = o.Key,
                    name = o.Value
                }).OrderBy(o => o.name), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Create(string name)
        {
            Business.Edits.Role role = Business.Edits.Role.Create();

            // Exception should be generated on BLL error, such as duplication.
            try
            {
                role.Name = name;
                role.Save();

                return Json(new
                {
                    id = role.ID,
                    name = role.Name
                });
            }
            catch (Csla.Rules.ValidationException e)
            {
                // Http Status Code conflict.
                Response.StatusCode = 409;
                return Json(new
                {
                    message = e.Message,
                    brokenRules = role.BrokenRulesCollection.Select(o => new
                    {
                        description = o.Description
                    })
                });
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YRMC.SecureLogin.Web.Controllers
{
    [RequireHttps]
    public class CategoryController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult List()
        {
            Business.Lists.Categories categories = Business.Lists.Categories.GetAll();

            return Json(categories.Select(o => new
            {
                id = o.ID,
                name = o.Name
            }).OrderBy(o => o.name), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Create(string name)
        {
            Business.Edits.Category category = Business.Edits.Category.Create();

            // Exception should be generated on BLL error, such as duplication.
            try
            {
                category.Name = name;
                category.Save();

                return Json(new
                {
                    id = category.ID
                });
            }
            catch (Csla.Rules.ValidationException e)
            {
                // Http Status Code conflict.
                Response.StatusCode = 409;
                return Json(new
                {
                    message = e.Message,
                    brokenRules = category.BrokenRulesCollection.Select(o => new
                    {
                        description = o.Description
                    })
                });
            }
        }
    }
}

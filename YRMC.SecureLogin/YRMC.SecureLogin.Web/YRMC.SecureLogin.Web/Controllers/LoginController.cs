using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Code.Utilities.Extensions;
using System.Runtime.InteropServices;

namespace YRMC.SecureLogin.Web.Controllers
{
    [RequireHttps]
    public class LoginController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Index(Guid entryID)
        {
            Business.Lists.Login login = Business.Lists.Login.GetByEntryID(entryID);

            return Json(new
            {
                id = login.ID,
                entryid = login.EntryID,
                categoryid = login.CategoryID,
                roleid = login.RoleID,
                username = login.Username,
                description = login.Description
            }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult List(string searchBy, string searchText)
        {
            Business.Searches.Logins logins = Business.Searches.Logins.GetBySearch(searchBy, searchText);

            return Json(logins                
                .Select(o => new
                {
                    entryid = o.EntryID,
                    categoryid = o.CategoryID,
                    categoryname = o.CategoryName,
                    rolename = o.RoleName,
                    description = o.Description,
                    username = o.Username,
                }), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Logs(Guid entryId)
        {
            Business.Searches.Logs logs = Business.Searches.Logs.GetByEntryID(entryId);
            return Json(logs.Select(o => new
                {
                    modifieddate = o.ModifiedDate.ToString(System.Xml.XmlDateTimeSerializationMode.RoundtripKind),
                    madeby = new System.Security.Principal.SecurityIdentifier(o.MadeBy).Translate(typeof(System.Security.Principal.NTAccount)).ToString(),
                    actiondone = o.ActionDone
                }), JsonRequestBehavior.AllowGet);
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Create(Guid categoryID, Guid roleID, string description, string username, string password)
        {
            Business.Edits.Login login = Business.Edits.Login.Create();

            try
            {
                login.EntryID = login.ID;
                login.CategoryID = categoryID;
                login.RoleID = roleID;
                login.Username = username;
                login.Password = password;
                login.Description = description;
                login.Active = true;
                login.Save();

                Business.Edits.Log.WriteEntry("New login created", roleID, login.ID, login.EntryID);

                // Return HTTP Status successful.
                return Json(new { 
                    id = login.ID,
                    message = "Login created."
                });
            }
            catch (Csla.Rules.ValidationException e)
            {
                // Http Status Code conflict.
                Response.StatusCode = 409;
                return Json(new
                {
                    message = e.Message,
                    brokenRules = login.BrokenRulesCollection.Select(o => new
                    {
                        description = o.Description
                    })
                });
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Password(Guid entryID)
        {

            try
            {
                Business.Lists.Login login = Business.Lists.Login.GetByEntryID(entryID);

                // Retrieve the password to ensure the user has access.
                string password = login.Password;

                Business.Edits.Log.WriteEntry("Password Viewed", Guid.Empty, login.ID, login.EntryID);
                
                return Json(new
                {
                    password = password
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Csla.Rules.ValidationException ex)
            {
                Response.StatusCode = 403;
                return Json(new
                {
                    message = "Unauthorized"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Update(Guid id, Guid categoryID, Guid roleID, string username, string password, string description)
        {
            Business.Edits.Login oldLogin = Business.Edits.Login.GetByID(id);
            Business.Edits.Login newLogin = Business.Edits.Login.Create();

            if (oldLogin.Active)
            {
                try
                {
                    newLogin.EntryID = oldLogin.EntryID;
                    newLogin.CategoryID = (categoryID == Guid.Empty) ? oldLogin.CategoryID : categoryID;
                    newLogin.RoleID = (roleID == Guid.Empty) ? oldLogin.RoleID : roleID;
                    newLogin.Description = description;
                    newLogin.Username = username;

                    if (password != "********" && !password.IsNullOrWhiteSpace())
                    {
                        newLogin.Password = password;
                    }
                    else
                    {
                        newLogin.Password = oldLogin.Password;
                    }

                    newLogin.Active = true;
                    newLogin.ModifiedDate = DateTime.UtcNow;

                    oldLogin.Active = false;

                    // Has anything changed?
                    if (oldLogin.CategoryID != newLogin.CategoryID ||
                        oldLogin.RoleID != newLogin.RoleID ||
                        oldLogin.Description != newLogin.Description ||
                        oldLogin.Username != newLogin.Username ||
                        oldLogin.Password != newLogin.Password)
                    {
                        // Deactivate the old record only if the new records writes successfully.
                        newLogin.Save();
                        oldLogin.Save();

                        Business.Edits.Log.WriteEntry("Login information changed", newLogin.RoleID, newLogin.ID, newLogin.EntryID);
                    }

                    // Return HTTP Status successful.
                    return Json(new
                    {
                        id = newLogin.ID,
                        message = "Login information changed."
                    });
                }
                catch (Csla.Rules.ValidationException e)
                {
                    // Http Status Code conflict.
                    Response.StatusCode = 409;
                    return Json(new
                    {
                        message = e.Message,
                        brokenRules = newLogin.BrokenRulesCollection.Select(o => new
                        {
                            description = o.Description
                        })
                    });
                }
            }
            else
            {
                // Return HTTP Status Error Code, loginID was already inactivated.
                Response.StatusCode = 409;
                return Json(new
                {
                    message = "Login was already inactivated.",
                    brokenRules = oldLogin.BrokenRulesCollection.Select(o => new
                    {
                        description = o.Description
                    })
                });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Delete(Guid loginID)
        {
            Business.Edits.Login login = Business.Edits.Login.GetByID(loginID);

            if (login.Active)
            {
                login.Active = false;
                login.Save();

                Business.Edits.Log.WriteEntry("Login deleted", Guid.Empty, loginID, login.EntryID);
                return Json(new
                {
                    message = "Login deleted."
                });
            }
            else
            {
                // Return HTTP Status Error Code, loginID was already deleted.
                Response.StatusCode = 409;
                return Json(new
                {
                    message = "Login was already deleted.",
                    brokenRules = login.BrokenRulesCollection.Select(o => new
                    {
                        description = o.Description
                    })
                });
            }
        }
    }
}

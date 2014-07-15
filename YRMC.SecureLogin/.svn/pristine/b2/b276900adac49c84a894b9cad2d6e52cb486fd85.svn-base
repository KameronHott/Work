using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Text;
using System.DirectoryServices;
using Code.Utilities.Extensions;

namespace YRMC.SecureLogin.Web.Controllers
{
    [RequireHttps]
    public class UserController : Controller
    {
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Index(Guid userId)
        {
            Business.Lists.User user = Business.Lists.User.GetByID(userId);
            return Json(new
            {
                id = user.ID,
                sid = user.SID,
                username = user.Username,
                active = user.Active,
                roles = (user.IsAdmin ?
                    user.Roles.Where(r => r.Name.ToLower() == "administrators").Select(r => new
                    {
                        id = r.ID,
                        name = r.Name
                    }) : user.Roles.Select(r => new
                    {
                        id = r.ID,
                        name = r.Name
                    })
                )
            }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Add(string username)
        {
            string userSID = null;

            try
            {
                DirectoryEntry directoryEntry = new DirectoryEntry(
                    string.Format(
                        "WinNT://{0}/{1}",
                        "YRMC_MAIN",
                        username),
                    System.Configuration.ConfigurationManager.AppSettings["LDAPUsername"],
                    System.Configuration.ConfigurationManager.AppSettings["LDAPPassword"]);

                userSID = new System.Security.Principal.SecurityIdentifier(
                    (byte[])directoryEntry.Properties["objectSid"].Value, 0).ToString();
            }
            catch {  }

            if (!userSID.IsNullOrWhiteSpace())
            {
                if (!Business.Lists.User.ExistsBySID(userSID))
                {
                    Business.Edits.User user = Business.Edits.User.Create();
                    user.SID = userSID;
                    user.Active = false;
                    user.Save();

                    // Return HTTP Status successful.
                    return Json(new
                    {
                        message = "User added."
                    });
                }
                else
                {
                    // Return HTTP Status failure.
                    Response.StatusCode = 409;
                    return Json(new
                    {
                        message = "User already exists."
                    });
                }
            }
            else
            {
                // Return HTTP Status failure.
                Response.StatusCode = 404;
                return Json(new
                {
                    message = "User could not be found in Active Directory."
                });
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult List()
        {
            Business.Lists.Users users = Business.Lists.Users.GetAll();

            return Json(users.Select(o => new
            {
                id = o.ID,
                sid = o.SID,
                username = o.Username,
                active = o.Active,
                roles = (o.IsAdmin ?
                    o.Roles.Where(r => r.Name.ToLower() == "administrators").Select(r => new
                    {
                        id = r.ID,
                        name = r.Name
                    }) : o.Roles.Select(r => new
                    {
                        id = r.ID,
                        name = r.Name
                    })
                )
            }), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Update(Guid id, Guid[] roles, bool active)
        {
            Business.Edits.User user = Business.Edits.User.GetByID(id);

            try
            {
                if (user.Active != active)
                {
                    user.Active = active;
                    user.Save();
                }

                Business.Lists.UserRoles userRoles = Business.Lists.UserRoles.GetByUserID(id);

                // Remove any potential Empty Guid's.
                roles = roles.Where(o => o != Guid.Empty).ToArray();

                // Delete
                foreach (Business.Lists.UserRole userRole in userRoles)
                {
                    bool found = false;

                    foreach (Guid role in roles)
                    {
                        if (role == userRole.ID)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        Business.Edits.UserRole delUserRole = Business.Edits.UserRole.GetByPIDRoleID(id, userRole.ID);
                        delUserRole.Delete();
                        delUserRole.Save();
                    }
                }

                // Add
                foreach (Guid role in roles)
                {
                    bool found = false;

                    foreach (Business.Lists.UserRole userRole in userRoles)
                    {
                        if (userRole.ID == role)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        Business.Edits.UserRole newUserRole = Business.Edits.UserRole.Create();
                        newUserRole.PID = id;
                        newUserRole.ID = role;
                        newUserRole.Save();
                    }
                }

                // Return HTTP Status successful.
                return Json(new
                {
                    message = "User updated."
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    message = e.Message,
                    brokenRules = user.BrokenRulesCollection.Select(o => new
                    {
                        description = o.Description
                    })
                });
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult Current()
        {
            try
            {
                Business.Security.SecurityIdentity currentUser =
                    (Business.Security.SecurityIdentity)Csla.ApplicationContext.User.Identity;

                return Json(new
                {
                    fullname = currentUser.Name,
                    isadmin = currentUser.IsAdmin
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Response.StatusCode = 403;
                return Json(new
                {
                    message = "Unauthorized"
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}

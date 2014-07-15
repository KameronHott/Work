using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.DirectoryServices;

namespace YRMC.Nursing.BedBoard.Web.Mvc.Controllers
{
    public class CommentsController : Controller
    {
        private NursingBedBoardEntities theDb;
        private Log dblog;
        //
        // GET: /Comments/
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Index(string patientaccount)
        {
            string thecomment = "";

            theDb = new NursingBedBoardEntities();
            if (theDb.Comments.Where(p => p.AccountNumber == patientaccount).Count() != 0)
            {
                var entities = new NursingBedBoardEntities();

                IQueryable<Comment> items = (
                        from row in entities.Comments
                        where row.AccountNumber == patientaccount
                        orderby row.CommentsID
                        select row);

                foreach (var comments in items)
                {
                    string dttime = comments.CommentDateTime.ToString("MM/dd/yy @ HH:mm");

                    thecomment += "<font style='color:blue'>" + GetFullName(comments.EnteredBy.ToString()) + " on " + dttime + "</font> - " + comments.CommentText.ToString() + "\r\n";
                }

            }

            return Json(new
            {
                Success = true,
                Message = thecomment
            });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ClearIt(string unit)
        {
            theDb = new NursingBedBoardEntities();
            
            var query = from p in theDb.Comments
                        where p.Unit == unit && p.AccountNumber == ""
                        select p;

            foreach (Comment c in query)
            {
                LogIt(c.HospitalNumber.ToString(), c.Unit.ToString(), c.AccountNumber.ToString(), "CCLEAR", "", "");
                theDb.DeleteObject(c);
            }

            theDb.SaveChanges();

            return Json(new
            {
                Success = true,
                //Message = thecomment
            });
        }

        public string GetFullName(string loginid)
        {
            try
            {
                DirectoryEntry dEntry = new DirectoryEntry("LDAP://dc=YRMC,dc=org");
                DirectorySearcher dSearcher = new DirectorySearcher(dEntry);
                dSearcher.Filter = "(&(objectCategory=person)(sAMAccountName=" + loginid + "))";
                DirectoryEntry result = dSearcher.FindOne().GetDirectoryEntry();
                return result.Properties["displayName"].Value.ToString();
            }
            catch { return loginid; }
        }

        public void LogIt(string hospitalnumber, string unit, string accountnumber, string logcode, string changemade, string commentsmade)
        {
            NursingBedBoardEntities thedb = new NursingBedBoardEntities();
            dblog = new Log();
            dblog.AccountNumber = accountnumber;
            dblog.ChangeMade = changemade;
            dblog.CommentsMade = commentsmade;
            dblog.EnteredDateTime = DateTime.Now;
            dblog.HospitalNumber = hospitalnumber;
            dblog.LogCode = logcode;
            dblog.Unit = unit;
            dblog.WhoMade = Request.ServerVariables.Get("AUTH_USER");
            thedb.Logs.AddObject(dblog);
            thedb.SaveChanges();
        }

    }
}

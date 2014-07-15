using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace YRMC.Nursing.BedBoard.Web.Mvc.Controllers
{
    public class NotationsController : Controller
    {

        private NursingBedBoardEntities theDb;
        private Notation dbN;
        private Comment dbC;
        private string accountnumber = "";
        private string hospitalnumber = "";
        private Boolean savecomment = false;
        private Boolean savepending = false;
        private Log dblog;

        static List<Notation> _notations = new List<Notation>();
        static List<Comment> _comments = new List<Comment>();


        //
        // POST: /Notations/Create
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Create(string patientname, string unit, string comment, string color, string roomtype)
        {
            try
            {
                if (SetKeyFields(unit))
                {
                    // Normal process for setting comments or flags
                    if (!IsNotated(hospitalnumber,accountnumber,unit) || roomtype=="isolation" || roomtype=="unavailable") //If the unit does not exist, then insert
                    {
                        theDb = new NursingBedBoardEntities();
                        //Changing Room Type, reset room first.
                        if (comment.Length > 0 && color == "yellow") { savepending = true; } // Saves a comment on the room level
                        if (roomtype == "isolation" || roomtype == "unavailable") { RoomAvailable(patientname, unit); }
                        // If color is red or purple and there is an account being passed, make a new record just for the room type
                        if (roomtype == "isolation" || roomtype == "unavailable")
                        {
                            dbN = new Notation();
                            dbN.Unit = unit;
                            dbN.PatientName = "";
                            dbN.AlertColor = color;
                            dbN.RoomType = roomtype;
                            dbN.EnteredDateTime = DateTime.Now;
                            dbN.AccountNumber = "";
                            dbN.HospitalNumber = hospitalnumber;
                            theDb.Notations.AddObject(dbN);
                            theDb.SaveChanges();
                            if (roomtype == "isolation")
                            {
                                LogIt(hospitalnumber, unit, "", "NISOLATE", roomtype, "");
                            }
                            else { LogIt(hospitalnumber, unit, "", "NUNAVIL", roomtype, ""); }
                        }
                        else { 
                        //Save the data in the database
                        dbN = new Notation();
                        dbN.Unit = unit;
                        dbN.PatientName = patientname;
                        dbN.AlertColor = color;
                        dbN.EnteredDateTime = DateTime.Now;
                        if (accountnumber == "n/a") { accountnumber = ""; }
                        dbN.AccountNumber = accountnumber;
                        dbN.HospitalNumber = hospitalnumber;
                        dbN.RoomType = "";
                        theDb.Notations.AddObject(dbN);
                        theDb.SaveChanges();
                        //LogIt(hospitalnumber, unit, accountnumber, "NCREATE", roomtype, comment);
                    }
                        
                        if (comment.Length > 0 && color != "yellow") { savecomment = true; } // Saves a comment on the patient level
                    }
                    else //Unit exists, do an update
                    { 
                        // do an update
                        theDb = new NursingBedBoardEntities();
                        Notation notes;
                        if (accountnumber == "n/a")
                        {
                            notes = theDb.Notations.Single(p => p.HospitalNumber == hospitalnumber && p.Unit == unit);
                            if (accountnumber == "n/a") { accountnumber = ""; }
                        }
                        else
                        {
                            notes = theDb.Notations.Single(p => p.HospitalNumber == hospitalnumber && p.AccountNumber == accountnumber);
                        }
                        notes.Unit = unit;
                        notes.PatientName = patientname;
                        if (color.Length == 0 && comment.Length == 0)
                        {
                            notes.AlertColor = "";
                        }
                        else if (color.Length == 0)
                        {
                            // A comment was made - don't erase the color
                            savecomment = true;
                        }
                        else if (color == "yellow")
                        {
                            // This is an admission alert
                            notes.AlertColor = color;
                            notes.CommentText = comment;
                        }
                        else
                        {
                            // A flag was put in - don't erase the comment
                            notes.AlertColor = color;
                        }
                        notes.EnteredDateTime = DateTime.Now;
                        notes.RoomType = roomtype;

                        theDb.SaveChanges();
                        //LogIt(hospitalnumber, unit, accountnumber, "NUPDATE", roomtype, comment);
                    }

                    if (savecomment)
                    {
                        string userinfo = User.Identity.Name; Request.ServerVariables.Get("AUTH_USER");
                        string userip = Request.ServerVariables.Get("REMOTE_ADDR");
                        //Save to the comment table
                        theDb = new NursingBedBoardEntities();
                        dbC = new Comment();
                        dbC.Unit = unit;
                        dbC.CommentDateTime = DateTime.Now;
                        dbC.AccountNumber = accountnumber;
                        dbC.HospitalNumber = hospitalnumber;
                        dbC.CommentText = comment;
                        dbC.EnteredBy =  userinfo.Replace("YRMC_MAIN\\","");
                        dbC.EnteredByIp = userip;
                        theDb.Comments.AddObject(dbC);
                        theDb.SaveChanges();
                        LogIt(hospitalnumber, unit, accountnumber, "CCOMMENT", "", comment);

                        savecomment = false;
                    }
                    
                    if (savepending)
                    {
                        theDb = new NursingBedBoardEntities();
                        Notation notes;
                        if (accountnumber == "")
                        {
                            notes = theDb.Notations.Single(p => p.HospitalNumber == hospitalnumber && p.Unit == unit && p.AlertColor=="yellow");
                        }
                        else
                        {
                            notes = theDb.Notations.Single(p => p.HospitalNumber == hospitalnumber && p.AccountNumber == accountnumber && p.AlertColor=="yellow");
                        }
                        notes.CommentText = comment;
                        theDb.SaveChanges();
                        LogIt(hospitalnumber, unit, accountnumber, "APENDING", "PENDING", comment);
                    }
                }

                //Return a success
                return Json(new
                {
                    Success = true,
                    //Message = "Data saved Successfully " + patientname + ' ' + unit + ' ' + comment + ' ' + color
                });
                
            }
            catch (Exception ex)
            {
                //throw ex;

                return Json(new
                {
                    Success = false,
                    Message = "Data NOT saved Successfully " + ex.Message.ToString()
                });
            }
        }
        
        public Boolean IsNotated(string hospitalnumber, string accountnumber, string unit)
        {
            theDb = new NursingBedBoardEntities();
            if (accountnumber != "n/a")
            {
                if (theDb.Notations.Where(p => p.HospitalNumber == hospitalnumber && p.AccountNumber == accountnumber).Count() == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (theDb.Notations.Where(p => p.HospitalNumber == hospitalnumber && p.Unit == unit && (p.RoomType != "unavailable" && p.RoomType != "isolation")).Count() == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public Boolean SetKeyFields(string unit)
        {
            theDb = new NursingBedBoardEntities();
            if (theDb.Boards.Where(b => b.Unit == unit).Count() != 0)
            {
                //Board keyfields = theDb.Boards.Single(p => p.Unit == unit);

                IQueryable<Board> items = (
                        from row in theDb.Boards
                        where row.Unit == unit
                        select row);

                try
                {
                    foreach (var record in items)
                    {
                        accountnumber = record.AccountNumber.ToString();
                        hospitalnumber = record.HospitalNumber.ToString();
                    }
                }
                catch
                {
                    accountnumber = "n/a";
                    foreach (var record in items)
                    {
                        hospitalnumber = record.HospitalNumber.ToString();
                    }
                }
                
                    return true;
                
            }
            else
            { return false; }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult NoAdmin(string patientname, string unit, string comment, string color)
        {
            theDb = new NursingBedBoardEntities();
            if (theDb.Notations.Where(a => a.Unit == unit).Count() != 0)
            {
                // Delete non patient room to clean up database
                var query1 = from p in theDb.Notations
                             where p.Unit == unit && p.AccountNumber == "" && (p.RoomType != "isolation" && p.RoomType != "unavailable")
                             select p;

                foreach (Notation c in query1)
                {
                    LogIt(c.HospitalNumber, c.Unit, c.AccountNumber, "NNOPEND", "Pending Admission Removed", comment);
                    theDb.DeleteObject(c);
                }

                theDb.SaveChanges();

                // Remove flag on patient room if there is one.
                var query2 = from p in theDb.Notations
                             where p.Unit == unit && p.AlertColor == "yellow"
                            select p;

                string hostpitalnumber = "";
                string unitnum = "";
                string accountnumber = "";
                foreach (Notation c in query2)
                {
                    hostpitalnumber = c.HospitalNumber;
                    unitnum = c.Unit;
                    accountnumber = c.AccountNumber;
                    c.AlertColor = "";
                    c.CommentText = "";
                }

                theDb.SaveChanges();
                LogIt(hostpitalnumber, unitnum, accountnumber, "NNOPEND", "Pending Admission Removed", comment);
            }
            //Return a success
            return Json(new
            {
                Success = true,
                //Message = "Data saved Successfully " + patientname + ' ' + unit + ' ' + comment + ' ' + color
            });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RoomAvailable(string patientname, string unit)
        {
            theDb = new NursingBedBoardEntities();
            if (theDb.Notations.Where(a => a.Unit == unit).Count() != 0)
            {

                var query = from p in theDb.Notations
                            where p.Unit == unit && (p.RoomType == "isolation" || p.RoomType == "unavailable")
                            select p;

                string hostpitalnumber = "";
                string unitnum = "";
                foreach (Notation c in query)
                {
                    hostpitalnumber = c.HospitalNumber;
                    unitnum = c.Unit;
                    theDb.DeleteObject(c);
                }

                theDb.SaveChanges();
                LogIt(hostpitalnumber, unitnum, "", "NAVAIL", "Room Type Now Available", "");
            }


            //Return a success
            return Json(new
            {
                Success = true,
                //Message = "Data saved Successfully " + patientname + ' ' + unit + ' ' + comment + ' ' + color
            });
        }

        public void LogIt(string hospitalnumber, string unit, string accountnumber, string logcode, string changemade, string commentsmade)
        {
            if (unit.Length > 0)
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
                dblog.WhoMade = User.Identity.Name; // Request.ServerVariables.Get("AUTH_USER");
                thedb.Logs.AddObject(dblog);
                thedb.SaveChanges();
            }
        }
    }
}

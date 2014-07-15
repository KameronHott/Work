using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Text;
using System.Threading;
using System.Net.Mail;

namespace YRMC.Nursing.BedBoard.Web.Mvc.Controllers
{
    public class BoardsController : Controller
    {
        public enum IsAuthorizedEnum
        {
            Unauthorized,
            EVS,
            ViewOnly,
            Staffing
        }

        #region Location Class

        private Log dblog;

        private class Location
        {
            public Location(string name) { Name = name; }

            public string Name { get; set; }
            public List<Department> Departments = new List<Department>();
            public DateTime LastUpdate { get; set; }
        }

        private class Department
        {
            public Department(string name) { Name = name; }

            public string Name { get; set; }
            public List<Unit> Units = new List<Unit>();
        }

        private class Unit
        {
            public Unit(string name, string flag, string roomtype, string flagcomment) { 
                Name = name;
                Flag = flag;
                FlagComment = flagcomment;
                RoomType = roomtype;
            }

            public string Name { get; set; }
            public Patient Patient { get; set; }
            public string Flag { get; set; }
            public string FlagComment { get; set; }
            public string RoomType { get; set; }
        }

        private class Patient
        {
            public Patient(string name, string gender, string age, string atp, string dischargeStatus, string dischargeStatusComment, string description, string accountnumber, string commenttext, Boolean isclean, string pendingcounter)
            {
                string descPrefix = "";

                Name = name;
                Gender = gender;
                Age = age;
                ATP = atp;
                DischargeStatus = dischargeStatus;
                AccountNumber = accountnumber;
                CommentText = commenttext;
                IsClean = isclean;
                PendingCounter = pendingcounter;


                description = Regex.Replace(description, @",\b", @", ");
                description = Regex.Replace(description, @"\\b", @" \ ");
                description = Regex.Replace(description, @"/\b", @" / ");

                if (!string.IsNullOrEmpty(atp))
                    descPrefix = "ATP: " + atp + "<br />";

                if (!string.IsNullOrEmpty(DischargeStatus) &&
                    !string.IsNullOrEmpty(dischargeStatusComment))
                    descPrefix += "DSTAT: " + dischargeStatusComment + "<br />";

                Description =
                    descPrefix +
                    description;
            }

            public string Name { get; set; }
            public string Gender { get; set; }
            public string Age { get; set; }
            public string ATP { get; set; }
            public string DischargeStatus { get; set; }
            public string Description { get; set; }
            public string AccountNumber { get; set; }
            public string CommentText { get; set; }
            public Boolean IsClean { get; set; }
            public string PendingCounter { get; set; }
           
        }

        #endregion

        static Dictionary<IsAuthorizedEnum, List<Location>> _locations = new Dictionary<IsAuthorizedEnum, List<Location>>();
        static TimeSpan _locationTimespan = new TimeSpan(0, 0, 10); // Allows updates every 10 seconds.

        //
        // GET: /Boards/
        [HttpGet()]
        public ActionResult Index(string id /* Location -> Name */)
        {
            IsAuthorizedEnum isAuthorized = IsAuthorized();
            bool update = false;
            Location location;

            if (isAuthorized == IsAuthorizedEnum.Unauthorized)
            {
                Response.StatusCode = 401;
                return Json(new { Error = "Unauthorized" }, JsonRequestBehavior.AllowGet);
            }

            if (!_locations.ContainsKey(isAuthorized))
                _locations.Add(isAuthorized, new List<Location>());

            if (_locations[isAuthorized].Where(o => o.Name == id).Count() == 0)
            {
                update = true;
            }
            else if (DateTime.Now.Subtract(_locations[isAuthorized].Where(o => o.Name == id).First().LastUpdate) >= _locationTimespan)
            {
                update = true;
            }

            if (update)
            {
                location = new Location(id);

                using (var entities = new NursingBedBoardEntities())
                {
                    if (String.IsNullOrEmpty(id))
                        return null;

                    List<Board> items = (
                        from row in entities.Boards
                        where row.Location == id
                        orderby row.Location, row.Department, row.Unit.Length, row.Unit
                        select row).ToList();

                    var departments = items.Where(o => o.Location == id).Select(o => o.Department).Distinct();

                    foreach (string departmentName in departments)
                    {
                        Department department = new Department(departmentName);
                        var units = items.Where(o => o.Location == id && o.Department == departmentName).Select(o => o.Unit).Distinct();

                        foreach (string unitName in units)
                        {

                            Unit unit;
                            string roomtype = "";

                            var item = items.Where(o =>
                                o.Location == id &&
                                o.Department == departmentName &&
                                o.Unit == unitName)
                                .Select(o => o).FirstOrDefault();

                            if (IsIsolation(item.HospitalNumber, item.Unit)) { roomtype = "isolation"; }
                            if (IsUnavailable(item.HospitalNumber, item.Unit)) { roomtype = "unavailable"; }

                            DateTime PendingDate;
                            string elapsedtime = "";
                            try
                            {
                                if (item.DischargeStatusDateTime.Length > 1)
                                {
                                    PendingDate = Convert.ToDateTime(item.DischargeStatusDateTime);
                                    TimeSpan ts = DateTime.Now.Subtract(PendingDate);
                                    if (ts.Days > 0)
                                    {
                                        elapsedtime = string.Format("{0}D {1:00}:{2:00}", ts.Days, ts.Hours, ts.Minutes);
                                    }
                                    else { elapsedtime = string.Format("{0:00}:{1:00}", ts.Hours, ts.Minutes); }
                                }
                            }
                            catch
                            { //Nothing, it is null 
                            }

                            if (string.IsNullOrEmpty(item.AccountNumber))
                            {
                                unit = new Unit(unitName, FlagColor(item.HospitalNumber, "::" + unitName, unitName), roomtype, GetFlagComment(item.HospitalNumber, unitName, ""));

                                if (isAuthorized == IsAuthorizedEnum.Staffing || isAuthorized == IsAuthorizedEnum.ViewOnly)
                                {
                                    unit.Patient = new Patient(item.Name, item.Gender, item.Age, item.ATP, item.DischargeStatus, item.DischargeStatusComment, item.Description, "", LastComment(item.HospitalNumber, "::" + unitName), item.IsClean, elapsedtime);
                                }
                                else
                                {
                                    unit.Patient = new Patient("", "", "", "", "", "", "", "", "", item.IsClean, elapsedtime);
                                }
                            }
                            else
                            {
                                if (isAuthorized == IsAuthorizedEnum.Staffing || isAuthorized == IsAuthorizedEnum.ViewOnly)
                                {
                                    if (FlagColor(item.HospitalNumber, item.AccountNumber, item.Unit).Length == 0)
                                    {
                                        unit = new Unit(unitName, "", roomtype, GetFlagComment(item.HospitalNumber, unitName, ""));
                                    }
                                    else
                                    {
                                        unit = new Unit(unitName, FlagColor(item.HospitalNumber, item.AccountNumber, item.Unit), roomtype, GetFlagComment(item.HospitalNumber, item.Unit, item.AccountNumber));
                                    }

                                    unit.Patient = new Patient(item.Name, item.Gender, item.Age, item.ATP, item.DischargeStatus, item.DischargeStatusComment, item.Description, item.AccountNumber.ToString(), LastComment(item.HospitalNumber, item.AccountNumber.ToString()), item.IsClean, elapsedtime);
                                }
                                else
                                {
                                    unit = new Unit(unitName, "", roomtype, "");

                                    if (item.Name.Length > 1)
                                    {
                                        unit.Patient = new Patient("Occupied", "", "", "", item.DischargeStatus, "", "", "", "", item.IsClean, elapsedtime);
                                    }
                                    else
                                    {
                                        unit.Patient = new Patient("", "", "", "", "", "", "", "", "", item.IsClean, elapsedtime);
                                    }
                                }
                            }

                            // Check if dirty and if notification needs to be sent.
                            DirtyNotification(unitName, item.HospitalNumber, item.Department);

                            department.Units.Add(unit);
                        }

                        location.Departments.Add(department);
                    }
                }

                location.LastUpdate = DateTime.Now;

                // Replace/Add the latest data in our static list.
                if (_locations[isAuthorized].Where(o => o.Name == id).Count() != 0)
                    _locations[isAuthorized][_locations[isAuthorized].IndexOf(_locations[isAuthorized].Where(o => o.Name == id).First())] = location;
                else
                    _locations[isAuthorized].Add(location);
            }
            else
            {
                location = _locations[isAuthorized].Where(o => o.Name == id).First();
            }
           
            return Json(location, JsonRequestBehavior.AllowGet);
        }

        private void DirtyNotification(string UnitName, string HospitalNumber, string Location)
        {
            var entities = new NursingBedBoardEntities();
            var query = from row in entities.Boards
                        where row.Unit == UnitName && row.HospitalNumber == HospitalNumber && row.IsClean == false && row.Name == ""
                        select row;
            try
            {
                Boolean SendIt = false;
                foreach (var note in query)
                {
                    if (!note.DirtyNotification.HasValue)
                    {
                        SendIt = true;
                    }
                    else if (note.DirtyNotification.Value.AddHours(4) < DateTime.Now)
                    {
                        SendIt = true;
                    }
                    if (SendIt)
                    {
                        // The email needs to be sent
                        // The DirtyNotification needs to be set
                        // Event needs to be logged
                        if ((DateTime.Today.DayOfWeek != DayOfWeek.Saturday && DateTime.Today.DayOfWeek != DayOfWeek.Sunday) || !(DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 6))
                        {

                            if (HospitalNumber == "1")
                            {
                                MailAddress from = new MailAddress("BedboardNotify@yrmc.org");
                                MailAddress to = new MailAddress(((HospitalNumber == "1") ? "evsbedboardwest@yrmc.org" : "evsbedboardwest@yrmc.org"));
                                MailMessage mail = new MailMessage(from, to);
                                mail.Subject = "Dirty room on " + Location;
                                mail.Body = "Room: " + UnitName + Environment.NewLine + ((HospitalNumber == "1") ? " West Campus" : " East Campus");
                                SmtpClient client = new SmtpClient("alert.yrmc.org");
                                try
                                {
                                    var entities2 = new NursingBedBoardEntities();
                                    var query2 = from row in entities2.Boards
                                                 where row.Unit == UnitName && row.HospitalNumber == HospitalNumber && row.IsClean == false
                                                 select row;

                                    foreach (var rec in query2)
                                    {
                                        rec.DirtyNotification = DateTime.Now;
                                    }
                                    entities2.SaveChanges();

                                    client.Send(mail);

                                    LogIt(HospitalNumber, UnitName, "", "NOTIFY", "Dirty room notification", "");
                                }
                                catch (Exception e) { throw e; }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        public string LastComment(string rhospitalnumber, string raccountnumber)
        {
            using (NursingBedBoardEntities theDb = new NursingBedBoardEntities())
            {
                // this is for a comment on an empty room
                if (raccountnumber.Substring(0, 2) == "::")
                {
                    var entities = new NursingBedBoardEntities();
                    string thecomment = "";

                    IQueryable<Comment> items = (
                            from row in entities.Comments
                            where row.HospitalNumber == rhospitalnumber && row.Unit == raccountnumber.Substring(2) && row.AccountNumber == ""
                            orderby row.CommentsID
                            select row);

                    try
                    {
                        foreach (var comment in items)
                        {
                            string dttime = comment.CommentDateTime.ToString("MM/dd/yy @ HH:mm");
                            thecomment = "Cmt By: " + GetFullName(comment.EnteredBy.ToString()) + " on " + dttime + " - " + comment.CommentText.ToString();
                        }
                        return thecomment.ToString();
                    }
                    catch
                    {
                        return "";
                    }
                }
                // this would leave a comment on a patient
                else if (theDb.Comments.Where(p => p.HospitalNumber == rhospitalnumber && p.AccountNumber == raccountnumber).Count() != 0)
                {
                    var entities = new NursingBedBoardEntities();
                    string thecomment = "";

                    IQueryable<Comment> items = (
                            from row in entities.Comments
                            where row.HospitalNumber == rhospitalnumber && row.AccountNumber == raccountnumber
                            orderby row.CommentsID
                            select row);

                    foreach (var comment in items)
                    {
                        string dttime = comment.CommentDateTime.ToString("MM/dd/yy @ HH:mm");
                        thecomment = "Cmt By: " + GetFullName(comment.EnteredBy.ToString()) + " on " + dttime + " - " + comment.CommentText.ToString();
                    }

                    return thecomment.ToString();
                }
                else
                { return ""; }
            }
        }

        public string FlagColor(string rhospitalnumber, string raccountnumber, string unit)
        {
            using (NursingBedBoardEntities theDb = new NursingBedBoardEntities())
            {
                // This is a flag for an empty room
                if (raccountnumber.Substring(0, 2) == "::")
                {
                    var entities = new NursingBedBoardEntities();
                    string thependingcolor = "";
                    string thecolor = "";
                    DateTime thedatetime = Convert.ToDateTime("1/2/1900");

                    IQueryable<Notation> items = (
                            from row in entities.Notations
                            where row.HospitalNumber == rhospitalnumber && row.Unit == raccountnumber.Substring(2) && row.AccountNumber == "" && (row.RoomType == null || row.RoomType == "")
                            select row);
                    try
                    {
                        foreach (var color in items)
                        {
                            if (color.AlertColor == "yellow")
                                thependingcolor = "yellow";
                            thedatetime = Convert.ToDateTime(color.EnteredDateTime);
                        }
                        thecolor = thependingcolor;
                        if (IsPendingAdmission(rhospitalnumber, unit, "") >= thedatetime) { thecolor = "yellow"; }
                        return thecolor.ToString();
                    }
                    catch
                    {
                        return "";
                    }
                }
                // This is a flag for a patient room
                else if (theDb.Notations.Where(p => p.HospitalNumber == rhospitalnumber && p.Unit == unit).Count() != 0)
                {
                    var entities = new NursingBedBoardEntities();
                    string thecolor = "";
                    DateTime thedatetime = Convert.ToDateTime("1/2/1900");

                    IQueryable<Notation> items = (
                            from row in entities.Notations
                            where row.HospitalNumber == rhospitalnumber && row.Unit == unit && (row.RoomType == null || row.RoomType == "")
                            select row);

                    foreach (var color in items)
                    {
                        thecolor = color.AlertColor;
                        thedatetime = Convert.ToDateTime(color.EnteredDateTime);
                    }

                    if (IsPendingAdmission(rhospitalnumber, unit, raccountnumber) >= thedatetime) { thecolor = "yellow"; }
                    return thecolor.ToString();
                }
                else
                { return ""; }
            }
        }

        public string GetFlagComment(string hospitalnumber, string unit, string accountnumber)
        {
            using (NursingBedBoardEntities entities = new NursingBedBoardEntities())
            {
                IQueryable<Notation> items = (
                        from row in entities.Notations
                        where row.HospitalNumber == hospitalnumber && row.Unit == unit && (row.RoomType == null || row.RoomType == "")
                        orderby row.EnteredDateTime descending
                        select row);

                foreach (var color in items)
                {
                    if (color.AlertColor == "yellow" && color.AccountNumber == accountnumber)
                    {
                        try
                        {
                            return color.CommentText.ToString();
                        }
                        catch { return ""; }
                    }
                    else if (color.AlertColor == "yellow" && accountnumber == "")
                    {
                        return color.CommentText.ToString();
                    }
                    else { return ""; }
                }

                return "";
            }
        }

        public Boolean IsIsolation(string hospitalnumber, string unit)
        {
            using (NursingBedBoardEntities entities = new NursingBedBoardEntities())
            {
                IQueryable<Notation> items = (
                            from row in entities.Notations
                            where row.HospitalNumber == hospitalnumber && row.Unit == unit && row.AccountNumber == "" && row.RoomType == "isolation"
                            select row);

                foreach (var color in items)
                {
                    return true;
                }

                return false;
            }            
        }

        public Boolean IsUnavailable(string hospitalnumber, string unit)
        {
            using (NursingBedBoardEntities entities = new NursingBedBoardEntities())
            {
                IQueryable<Notation> items = (
                            from row in entities.Notations
                            where row.HospitalNumber == hospitalnumber && row.Unit == unit && row.AccountNumber == "" && row.RoomType == "unavailable"
                            select row);

                foreach (var color in items)
                {
                    return true;
                }

                return false;
            }
        }

        public DateTime IsPendingAdmission(string hospitalnumber,string unit, string accountnumber)
        {
            using (NursingBedBoardEntities entities = new NursingBedBoardEntities())
            {
                IQueryable<Notation> items = (
                            from row in entities.Notations
                            where row.HospitalNumber == hospitalnumber && row.Unit == unit && (row.RoomType == null || row.RoomType == "")
                            orderby row.EnteredDateTime descending
                            select row);

                foreach (var color in items)
                {
                    if (color.AlertColor == "yellow" && color.AccountNumber == accountnumber)
                    {
                        return Convert.ToDateTime(color.EnteredDateTime);
                    }
                    else if (color.AlertColor == "yellow" && accountnumber == "")
                    {
                        return Convert.ToDateTime(color.EnteredDateTime);
                    }
                    else { return Convert.ToDateTime("1/1/1900"); }
                }

                return Convert.ToDateTime("1/1/1900");
            }
        }

        [HttpPost]
        public JsonResult RoomClean(string unit)
        {
            using (NursingBedBoardEntities theDb = new NursingBedBoardEntities())
            {
                if (theDb.Boards.Where(a => a.Unit == unit).Count() != 0)
                {
                    // Change IsClean flag to 1 (true)
                    var query2 = from p in theDb.Boards
                                 where p.Unit == unit
                                 select p;
                    string hostpitalnumber = "";
                    string unitnum = "";
                    string department = "";
                    foreach (Board c in query2)
                    {
                        hostpitalnumber = c.HospitalNumber;
                        unitnum = c.Unit;
                        department = c.Department;
                        c.IsClean = true;
                        c.DirtyNotification = null;
                    }
                    LogIt(hostpitalnumber, unitnum, "", "BCLEAN", "Room Is Clean", "");
                    MailAddress from = new MailAddress("BedboardNotify@yrmc.org");
                    MailAddress to = new MailAddress("BedboardNurseStaffing@yrmc.org");
                    MailMessage mail = new MailMessage(from, to);
                    mail.Subject = "Room cleaned on " + department;
                    mail.Body = "Room: " + unitnum + Environment.NewLine + ((hostpitalnumber == "1") ? " West Campus" : " East Campus");
                    SmtpClient client = new SmtpClient("alert.yrmc.org");
                    client.Send(mail);
                }
                theDb.SaveChanges();

                //Return a success
                return Json(new
                {
                    Success = true,
                    //Message = "Data saved Successfully " + patientname + ' ' + unit + ' ' + comment + ' ' + color
                });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetLoginId()
        {
            //string userSID = GetSid(Request.ServerVariables.Get("AUTH_USER")).ToString();
            //string userSID = GetSid("YRMC_MAIN\\tajackso").ToString();
            IsAuthorizedEnum isAuthorized = IsAuthorized();
            return Json(new
            {
                Success = true,
                Role = (
                    isAuthorized == IsAuthorizedEnum.EVS ?
                        "EVS" : isAuthorized == IsAuthorizedEnum.ViewOnly ?
                            "VO" : isAuthorized == IsAuthorizedEnum.Staffing ?
                                "Staffing" : "")
            });
        }

        private IsAuthorizedEnum IsAuthorized()
        {
            // Test authorization level
            bool isInStaffing = false;
            bool isInEVS = false;
            bool isInVO = false;
            string userName = ((System.Security.Principal.WindowsIdentity)(User.Identity)).Name;
            using (var ctx = new PrincipalContext(ContextType.Domain, "YRMC.ORG"))
            {
                //Staffing Office
                using (var grp = GroupPrincipal.FindByIdentity(ctx, IdentityType.Sid, "S-1-5-21-999033763-294680432-740312968-18279"))
                {
                    isInStaffing = grp != null &&
                        grp
                        .GetMembers(true)
                        .Any(m => m.SamAccountName == userName.Replace("YRMC_MAIN" + "\\", ""));
                }
                //EVS
                using (var grp = GroupPrincipal.FindByIdentity(ctx, IdentityType.Sid, "S-1-5-21-999033763-294680432-740312968-22488"))
                {
                    isInEVS = grp != null &&
                        grp
                        .GetMembers(true)
                        .Any(m => m.SamAccountName == userName.Replace("YRMC_MAIN" + "\\", ""));
                }
                //View Only
                using (var grp = GroupPrincipal.FindByIdentity(ctx, IdentityType.Sid, "S-1-5-21-999033763-294680432-740312968-22489"))
                {
                    isInVO = grp != null &&
                        grp
                        .GetMembers(true)
                        .Any(m => m.SamAccountName == userName.Replace("YRMC_MAIN" + "\\", ""));
                }
            }


            /*
            if (isInEVS) { _sauthorized = "false"; _srole = "EVS"; _authorized = false; }
            if (isInVO) { _sauthorized = "true"; _srole = "VO"; _authorized = true; }
            if (isInStaffing) { _sauthorized = "true"; _srole = "Staffing"; _authorized = true; }
            */

            if (isInEVS) return IsAuthorizedEnum.EVS;
            if (isInVO) return IsAuthorizedEnum.ViewOnly;
            if (isInStaffing) return IsAuthorizedEnum.Staffing;

            return IsAuthorizedEnum.Unauthorized;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RoomDirty(string unit)
        {
            using (NursingBedBoardEntities theDb = new NursingBedBoardEntities())
            {
                if (theDb.Boards.Where(a => a.Unit == unit).Count() != 0)
                {
                    // Change IsClean flag to 1 (true)
                    var query2 = from p in theDb.Boards
                                 where p.Unit == unit
                                 select p;
                    string hostpitalnumber = "";
                    string unitnum = "";
                    foreach (Board c in query2)
                    {
                        hostpitalnumber = c.HospitalNumber;
                        unitnum = c.Unit;
                        c.IsClean = false;
                        c.DirtyNotification = null;
                    }
                    LogIt(hostpitalnumber, unitnum, "", "BDIRTY", "Room Is Dirty", "");
                }
                
                theDb.SaveChanges();
             
                //Return a success
                return Json(new
                {
                    Success = true,
                    //Message = "Data saved Successfully " + patientname + ' ' + unit + ' ' + comment + ' ' + color
                });
            }
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
            using (NursingBedBoardEntities thedb = new NursingBedBoardEntities())
            {
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


        private string GetSid(string strLogin)
        {
            string str = "";
            // Parse the string to check if domain name is present.
            int idx = strLogin.IndexOf('\\');
            if (idx == -1)
            {
                idx = strLogin.IndexOf('@');
            }

            string strDomain;
            string strName;

            if (idx != -1)
            {
                strDomain = strLogin.Substring(0, idx);
                strName = strLogin.Substring(idx + 1);
            }
            else
            {
                strDomain = Environment.MachineName;
                strName = strLogin;
            }


            DirectoryEntry obDirEntry = null;
            try
            {
                Int64 iBigVal = 5;
                Byte[] bigArr = BitConverter.GetBytes(iBigVal);
                obDirEntry = new DirectoryEntry("WinNT://" + strDomain + "/" + strName);
                System.DirectoryServices.PropertyCollection coll = obDirEntry.Properties;
                object obVal = coll["objectSid"].Value;
                if (null != obVal)
                {
                    str = this.ConvertByteToStringSid((Byte[])obVal);
                }

            }
            catch (Exception ex)
            {
                str = "";
                System.Diagnostics.Trace.Write(ex.Message);
            }
            return str;
        }

        private string ConvertByteToStringSid(Byte[] sidBytes)
        {
            short sSubAuthorityCount = 0;
            StringBuilder strSid = new StringBuilder();
            strSid.Append("S-");
            try
            {
                // Add SID revision.
                strSid.Append(sidBytes[0].ToString());

                sSubAuthorityCount = Convert.ToInt16(sidBytes[1]);

                // Next six bytes are SID authority value.
                if (sidBytes[2] != 0 || sidBytes[3] != 0)
                {
                    string strAuth = String.Format("0x{0:2x}{1:2x}{2:2x}{3:2x}{4:2x}{5:2x}",
                                (Int16)sidBytes[2],
                                (Int16)sidBytes[3],
                                (Int16)sidBytes[4],
                                (Int16)sidBytes[5],
                                (Int16)sidBytes[6],
                                (Int16)sidBytes[7]);
                    strSid.Append("-");
                    strSid.Append(strAuth);
                }
                else
                {
                    Int64 iVal = (Int32)(sidBytes[7]) +
                            (Int32)(sidBytes[6] << 8) +
                            (Int32)(sidBytes[5] << 16) +
                            (Int32)(sidBytes[4] << 24);
                    strSid.Append("-");
                    strSid.Append(iVal.ToString());
                }

                // Get sub authority count...
                int idxAuth = 0;
                for (int i = 0; i < sSubAuthorityCount; i++)
                {
                    idxAuth = 8 + i * 4;
                    UInt32 iSubAuth = BitConverter.ToUInt32(sidBytes, idxAuth);
                    strSid.Append("-");
                    strSid.Append(iSubAuth.ToString());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.Write(ex.Message);
                return "";
            }
            return strSid.ToString();
        }

    }
}

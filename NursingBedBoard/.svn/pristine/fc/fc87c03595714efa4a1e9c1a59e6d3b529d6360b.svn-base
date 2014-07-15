using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace YRMC.Nursing.BedBoard.Web.Mvc.Controllers
{
    public class LocationsController : Controller
    {
        //
        // GET: /Departments/
        [HttpGet()]
        public ActionResult Index(string id)
        {
            List<Location> items;

            using (var entities = new NursingBedBoardEntities())
            {
                if (String.IsNullOrWhiteSpace(id))
                    items = (
                        from row in entities.Locations
                        where row.Active == true
                        orderby row.Order
                        select row).ToList();
                else
                    items = (
                        from row in entities.Locations
                        where row.Active == true &&
                            row.Name == id
                        orderby row.Order
                        select row).ToList();
            }

            return Json(items.Select(o => new {
                Name = o.Name,
                DisplayName= o.DisplayName
            }), JsonRequestBehavior.AllowGet);
        }
    }
}

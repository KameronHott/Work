using System.Web.Mvc;

namespace YRMC.Nursing.BedBoard.Web.Mvc.Areas.App_Code
{
    public class App_CodeAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "App_Code";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "App_Code_default",
                "App_Code/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

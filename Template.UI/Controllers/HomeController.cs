namespace Template.UI.Controllers
{
    using System.Web.Mvc;    
    using Incoding.MvcContrib;

    public class HomeController : IncControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Sample()
        {
            return View();
        }


        public ActionResult Custom_Engine()
        {
            return View();
        }
    }
}
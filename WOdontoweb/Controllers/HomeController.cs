namespace WOdontoweb.Controllers
{
    using System;
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

                return View();
            }
            catch (Exception) { throw; }
        }

        public ActionResult About()
        {
            try
            {
                ViewBag.Message = "Your app description page.";

                return View();
            }
            catch (Exception) { throw; }
        }

        public ActionResult Contact()
        {
            try
            {
                ViewBag.Message = "Your contact page.";

                return View();
            }
            catch (Exception) { throw; }
        }
    }
}
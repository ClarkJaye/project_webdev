using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Webdev_project.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Login()
        {
            ViewBag.Message = "Your Login page.";

            return View();
        }

        public ActionResult Register()
        {
            ViewBag.Message = "Your Register page.";

            return View();
        }



    }
}
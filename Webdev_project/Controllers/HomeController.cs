using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdev_project.Models;

namespace Webdev_project.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ProfilePage()
        {
            return View();
        }

        public ActionResult LoginFailed()
        {
            return View();
        }



        [HttpPost]
        public ActionResult AddUserToDatabase(FormCollection fc)
        {
            String firstName = fc["firstname"];
            String lastName = fc["lastname"];
            String gender = fc["gender"];
            String email = fc["email"];
            String address = fc["address"];
            String password = fc["password"];
            String confirmPassword = fc["confirm-password"];

            //var redirectPage = "";
        
            if (password == confirmPassword)
            {
                user_table use = new user_table();
                use.firstname = firstName;
                use.lastname = lastName;
                use.gender = gender;
                use.email = email;
                use.address = address;
                use.password = password;
                use.role_Id = 1;

                ActivityhubEntities2 fe = new ActivityhubEntities2();
                fe.user_table.Add(use);
                fe.SaveChanges();

                return RedirectToAction("ProfilePage");

            }


            return RedirectToAction("LoginFailed");
        }



        public ActionResult LoginAndRedirect(FormCollection gd)
        {
            string email = gd["email"];
            string password = gd["password"];

            using (var dbContext = new ActivityhubEntities2())
            {
                var user = dbContext.user_table.FirstOrDefault(a => a.email == email && a.password == password);

                if (user != null)
                {
                    return RedirectToAction("ProfilePage");
                }
            }

            return RedirectToAction("LoginFailed");
        }

        [HttpPost]
        public ActionResult Logout()
        {

            return RedirectToAction("Index", "Home");
        }





    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdev_project.Models;
using System.Data.Entity;

using Newtonsoft.Json;

namespace Webdev_project.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult AdminPage()
        {
            int userId = (int)Session["UserId"];

            using (var dbContext = new ActivityhubEntities2())
            {
                var user = dbContext.user_table.FirstOrDefault(a => a.user_Id == userId);

                if (user != null)
                {


                    return View(user);
                }
            }

            return RedirectToAction("LoginFailed");
        }



        public ActionResult UserList()
        {
            int userId = (int)Session["UserId"];

            using (var dbContext = new ActivityhubEntities2())
            {
                var userList = dbContext.user_table
                    .Where(a => a.user_Id != userId)
                    .Include(u => u.role_table)
                    .ToList();

                return View(userList);
            }
        }


        public ActionResult ProfilePage()
        {
            int userId = (int)Session["UserId"];

            using (var dbContext = new ActivityhubEntities2())
            {
                var user = dbContext.user_table.Include("role_table").FirstOrDefault(a => a.user_Id == userId);

                if (user != null)
                {
                    return View(user);
                }
            }

            return RedirectToAction("LoginFailed");
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
                // Store user ID in session
                    Session["UserId"] = user.user_Id;
                    Session["Role"] = user.role_Id;

                return RedirectToAction("ProfilePage");

            }


            return RedirectToAction("LoginFailed");
        }


        [HttpPost]
        public ActionResult LoginAndRedirect(FormCollection gd)
        {
            string email = gd["email"];
            string password = gd["password"];

            using (var dbContext = new ActivityhubEntities2())
            {
                var user = dbContext.user_table.FirstOrDefault(a => a.email == email && a.password == password);

                if (user != null)
                {
                    // Store user ID in session
                    Session["UserId"] = user.user_Id;
                    Session["Role"] = user.role_Id;

                    if (Convert.ToInt32(Session["Role"]) == 1)
                    {
                        return RedirectToAction("AdminPage");
                    }
                    else if (Convert.ToInt32(Session["Role"]) == 2)
                    {
                        return RedirectToAction("ProfilePage");
                    }

                    TempData["SuccessMessage"] = "Update Successfully!";

                }
            }

            return RedirectToAction("LoginFailed");
        }

        [HttpPost]
        public ActionResult UpdateUser(FormCollection fc)
        {
            int userId = (int)Session["UserId"];

            using (var dbContext = new ActivityhubEntities2())
            {
                var user = dbContext.user_table.FirstOrDefault(a => a.user_Id == userId);

                if (user != null)
                {
                    string oldPassword = fc["old-password"];
                    string newPassword = fc["new_password"];

                    if (oldPassword == user.password && (newPassword != "" || newPassword != null))
                    {
                        user.firstname = fc["firstname"];
                        user.lastname = fc["lastname"];
                        user.gender = fc["gender"];
                        user.email = fc["email"];
                        user.address = fc["address"];

                        if (!string.IsNullOrEmpty(newPassword))
                        {
                            // Update the password if a new password is provided
                            user.password = newPassword;
                        }

                        dbContext.SaveChanges();
                        TempData["SuccessMessage"] = "Update Successfully!";

                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Invalid old password!";
                        return RedirectToAction("ProfilePage");
                    }
                }
            }

            return RedirectToAction("ProfilePage");
        }


        [HttpPost]
        public ActionResult UpdateUserList(FormCollection fc)
        {
            int userId = Convert.ToInt32(fc["user_Id"]);

            using (var dbContext = new ActivityhubEntities2())
            {
                var user = dbContext.user_table.FirstOrDefault(a => a.user_Id == userId);

                if (user != null)
                {
                        user.firstname = fc["firstname"];
                        user.lastname = fc["lastname"];
                        user.gender = fc["gender"];
                        user.email = fc["email"];
                        user.address = fc["address"];
                        user.password = fc["password"];
                        //user.role_Id = fc["role"];


                    dbContext.SaveChanges();
                        TempData["SuccessMessage"] = "Update Successfully!";

                }
            }

            return RedirectToAction("UserList");
        }




        [HttpPost]
        public ActionResult Logout()
        {
            // Show a flag indicating that the confirmation prompt should be displayed
            TempData["ShowLogoutConfirmation"] = true;

            return RedirectToAction("AdminPage");
        }

        public ActionResult LogoutConfirmed()
        {
            // Clear session variables
            Session.Clear();

            // Abandon the session
            Session.Abandon();

            return RedirectToAction("Index");
        }


        public ActionResult DeleteUser(int userId)
        {
            using (var dbContext = new ActivityhubEntities2())
            {
                var user = dbContext.user_table.FirstOrDefault(a => a.user_Id == userId);

                if (user != null)
                {
                    dbContext.user_table.Remove(user);
                    dbContext.SaveChanges();
                }
            }

            return RedirectToAction("UserList");
        }



        [HttpPost]
        public ActionResult EditUser(int userId)
        {
            using (var dbContext = new ActivityhubEntities2())
            {
                var user = dbContext.user_table.FirstOrDefault(a => a.user_Id == userId);

                if (user != null)
                {
                    // Pass the user object to the EditUser view for editing
                    return View("UpdateUserLists", user);
                }
            }

            return RedirectToAction("UserList");
        }

        
        public ActionResult GetUser(int userId)
        {
            using (var dbContext = new ActivityhubEntities2())
            {
                var user = dbContext.user_table.FirstOrDefault(a => a.user_Id == userId);

                if (user != null)
                {
                    // Return the user data as JSON
                    return Json(new
                    {
                        userId = user.user_Id,
                        firstname = user.firstname,
                        lastname = user.lastname,
                        gender = user.gender,
                        address = user.address,
                        email = user.email,
                        password = user.password,
                        roleId = user.role_Id
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(null); 
        }

    }
}

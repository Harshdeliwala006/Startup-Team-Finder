
using System;
using System.Linq;
using System.Web.Mvc;
using demostartupfinder.Models;

namespace demostartupfinder.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationDbContext db =
            new ApplicationDbContext();

        // GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var user = db.Users.FirstOrDefault(x =>
                x.Email == email &&
                x.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            Session["UserId"] = user.Id;
            Session["UserName"] = user.Name;
            Session["Role"] = user.Role;

            if (user.Role == "Admin")
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            return RedirectToAction("Dashboard", "Profile");
        }

        // GET: Account/Register
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (db.Users.Any(x => x.Email == user.Email))
            {
                ModelState.AddModelError(
                    "Email",
                    "This email is already registered.");
            }

            if (ModelState.IsValid)
            {
                user.Role = "User";
                user.CreatedAt = DateTime.Now;

                db.Users.Add(user);
                db.SaveChanges();

                TempData["Success"] =
                    "Registration successful! Please login.";

                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: Account/ForgotPassword
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Login", "Account");
        }
    }
}

using System.Linq;
using System.Web.Mvc;
using demostartupfinder.Models;

namespace demostartupfinder.Controllers
{
    public class ProfileController : Controller
    {
        private ApplicationDbContext db =
            new ApplicationDbContext();

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult MyProfile()
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int id = (int)Session["UserId"];

            var user = db.Users.Find(id);

            return View(user);
        }

        public ActionResult EditProfile()
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int id = (int)Session["UserId"];

            var user = db.Users.Find(id);

            return View(user);
        }

        [HttpPost]
        public ActionResult EditProfile(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State =
                    System.Data.Entity.EntityState.Modified;

                db.SaveChanges();

                TempData["Success"] =
                    "Profile updated successfully.";

                return RedirectToAction("MyProfile");
            }

            return View(user);
        }

        // GET: Profile/Skills
        public ActionResult Skills()
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int userId = (int)Session["UserId"];

            var skills = db.Skills
                .Where(x => x.UserId == userId)
                .ToList();

            return View(skills);
        }

        // POST: Profile/AddSkill
        [HttpPost]
        public ActionResult AddSkill(string skillName)
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            if (!string.IsNullOrWhiteSpace(skillName))
            {
                int userId =
                    (int)Session["UserId"];

                bool exists = db.Skills.Any(x =>
                    x.UserId == userId &&
                    x.SkillName == skillName);

                if (!exists)
                {
                    Skill skill = new Skill();

                    skill.SkillName =
                        skillName.Trim();

                    skill.UserId = userId;

                    db.Skills.Add(skill);

                    db.SaveChanges();
                }
            }

            return RedirectToAction("Skills");
        }

        // GET: Profile/DeleteSkill
        public ActionResult DeleteSkill(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int userId = (int)Session["UserId"];

            var skill = db.Skills.FirstOrDefault(x =>
                x.Id == id &&
                x.UserId == userId);

            if (skill != null)
            {
                db.Skills.Remove(skill);
                db.SaveChanges();
            }

            return RedirectToAction("Skills");
        }

        public ActionResult Settings()
        {
            return View();
        }
    }
}
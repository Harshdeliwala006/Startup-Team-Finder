using System.Linq;
using System.Web.Mvc;
using demostartupfinder.Models;

namespace demostartupfinder.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db =
            new ApplicationDbContext();

        private bool IsAdmin()
        {
            return Session["UserId"] != null &&
                   Session["Role"] != null &&
                   Session["Role"].ToString() == "Admin";
        }

        public ActionResult Dashboard()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            ViewBag.Users =
                db.Users.Count();

            ViewBag.Ideas =
                db.Ideas.Count();

            ViewBag.Teams =
                db.Teams.Count();

            ViewBag.JoinRequests =
                db.JoinRequests.Count();

            ViewBag.Messages =
                db.Messages.Count();

            ViewBag.Notifications =
                db.Notifications.Count();

            return View();
        }

        public ActionResult ManageUsers()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var users =
                db.Users
                .OrderBy(x => x.Name)
                .ToList();

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUser(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            int currentAdminId =
                (int)Session["UserId"];

            // Do not allow admin to delete himself
            if (id == currentAdminId)
            {
                TempData["Error"] =
                    "You cannot delete your own admin account.";

                return RedirectToAction("ManageUsers");
            }

            var user = db.Users.Find(id);

            if (user == null)
                return HttpNotFound();

            // Delete user's skills
            var skills =
                db.Skills
                .Where(x => x.UserId == id)
                .ToList();

            db.Skills.RemoveRange(skills);


            // Delete user's saved ideas
            var savedIdeas =
                db.SavedIdeas
                .Where(x => x.UserId == id)
                .ToList();

            db.SavedIdeas.RemoveRange(savedIdeas);


            // Delete user's join requests
            var joinRequests =
                db.JoinRequests
                .Where(x => x.UserId == id)
                .ToList();

            db.JoinRequests.RemoveRange(joinRequests);


            // Delete user's notifications
            var notifications =
                db.Notifications
                .Where(x => x.UserId == id)
                .ToList();

            db.Notifications.RemoveRange(notifications);


            // Delete messages sent by user
            var sentMessages =
                db.Messages
                .Where(x => x.SenderId == id)
                .ToList();

            db.Messages.RemoveRange(sentMessages);


            // Delete messages received by user
            var receivedMessages =
                db.Messages
                .Where(x => x.ReceiverId == id)
                .ToList();

            db.Messages.RemoveRange(receivedMessages);


            // Delete team memberships
            var memberships =
                db.TeamMembers
                .Where(x => x.UserId == id)
                .ToList();

            db.TeamMembers.RemoveRange(memberships);


            // Delete ideas created by user
            var ideas =
                db.Ideas
                .Where(x => x.UserId == id)
                .ToList();

            foreach (var idea in ideas)
            {
                var ideaSaved =
                    db.SavedIdeas
                    .Where(x => x.IdeaId == idea.Id)
                    .ToList();

                db.SavedIdeas.RemoveRange(ideaSaved);

                var ideaRequests =
                    db.JoinRequests
                    .Where(x => x.IdeaId == idea.Id)
                    .ToList();

                db.JoinRequests.RemoveRange(ideaRequests);
            }

            db.Ideas.RemoveRange(ideas);


            // Delete teams owned by user
            var teams =
                db.Teams
                .Where(x => x.OwnerId == id)
                .ToList();

            foreach (var team in teams)
            {
                var teamMembers =
                    db.TeamMembers
                    .Where(x => x.TeamId == team.Id)
                    .ToList();

                db.TeamMembers.RemoveRange(teamMembers);
            }

            db.Teams.RemoveRange(teams);


            // Finally delete user
            db.Users.Remove(user);

            db.SaveChanges();

            TempData["Success"] =
                "User deleted successfully.";

            return RedirectToAction("ManageUsers");
        }
    }
}
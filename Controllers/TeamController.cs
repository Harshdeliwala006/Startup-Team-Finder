using System;
using System.Linq;
using System.Web.Mvc;
using demostartupfinder.Models;

namespace demostartupfinder.Controllers
{
    public class TeamController : Controller
    {
        private ApplicationDbContext db =
            new ApplicationDbContext();

        public ActionResult MyTeams()
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int userId =
                (int)Session["UserId"];

            var teams = db.Teams
                .Where(x =>
                    x.OwnerId == userId ||
                    db.TeamMembers.Any(m =>
                        m.TeamId == x.Id &&
                        m.UserId == userId))
                .ToList();

            return View(teams);
        }

        public ActionResult Details(int id)
        {
            var team = db.Teams
                .Include("Idea")
                .FirstOrDefault(x => x.Id == id);

            if (team == null)
                return HttpNotFound();

            var members = db.TeamMembers
                .Include("User")
                .Where(x => x.TeamId == id)
                .ToList();

            ViewBag.Members = members;

            var owner = db.Users.Find(team.OwnerId);

            ViewBag.Owner = owner;

            bool isOwner = false;

            if (Session["UserId"] != null)
            {
                int currentUserId = (int)Session["UserId"];

                isOwner = currentUserId == team.OwnerId;
            }

            ViewBag.IsOwner = isOwner;

            return View(team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveMember(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int ownerId = (int)Session["UserId"];

            var member = db.TeamMembers
                .Include("Team")
                .Include("User")
                .FirstOrDefault(x => x.Id == id);

            if (member == null)
                return HttpNotFound();

            if (member.Team.OwnerId != ownerId)
            {
                TempData["Error"] =
                    "Only the team owner can remove members.";

                return RedirectToAction(
                    "Details",
                    new { id = member.TeamId });
            }

            int teamId = member.TeamId;

            Notification notification = new Notification();

            notification.UserId = member.UserId;

            notification.Message =
                "You have been removed from the team '" +
                member.Team.TeamName +
                "'.";

            notification.IsRead = false;

            notification.CreatedAt = DateTime.Now;

            db.Notifications.Add(notification);

            db.TeamMembers.Remove(member);

            db.SaveChanges();

            TempData["Success"] =
                "Team member removed successfully.";

            return RedirectToAction(
                "Details",
                new { id = teamId });
        }

        public ActionResult JoinRequests()
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int userId =
                (int)Session["UserId"];

            var requests = db.JoinRequests
                .Include("Idea")
                .Include("User")
                .Where(x =>
                    x.Idea.UserId == userId)
                .OrderByDescending(
                    x => x.RequestDate)
                .ToList();

            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AcceptRequest(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int ownerId =
                (int)Session["UserId"];

            var request = db.JoinRequests
                .Include("Idea")
                .FirstOrDefault(x =>
                    x.Id == id &&
                    x.Idea.UserId == ownerId);

            if (request == null)
                return HttpNotFound();

            if (request.Status != "Pending")
                return RedirectToAction(
                    "JoinRequests");

            // Find existing team for this idea
            var team = db.Teams.FirstOrDefault(
                x => x.IdeaId == request.IdeaId);

            // If no team exists, create one
            if (team == null)
            {
                team = new Team();

                team.TeamName =
                    request.Idea.Title + " Team";

                team.Description =
                    "Team for " +
                    request.Idea.Title;

                team.IdeaId =
                    request.IdeaId;

                team.OwnerId =
                    ownerId;

                db.Teams.Add(team);

                db.SaveChanges();

                // Add team owner as Founder
                TeamMember ownerMember = new TeamMember();

                ownerMember.TeamId = team.Id;
                ownerMember.UserId = ownerId;
                ownerMember.Position = "Founder";

                db.TeamMembers.Add(ownerMember);

                db.SaveChanges();
            }

            // Check if user already belongs
            bool alreadyMember =
                db.TeamMembers.Any(x =>
                    x.TeamId == team.Id &&
                    x.UserId == request.UserId);

            if (!alreadyMember)
            {
                TeamMember member =
                    new TeamMember();

                member.TeamId =
                    team.Id;

                member.UserId =
                    request.UserId;

                member.Position =
                    "Team Member";

                db.TeamMembers.Add(member);
            }

            request.Status =
                "Accepted";

            // Notification to requester
            Notification notification =
                new Notification();

            notification.UserId =
                request.UserId;

            notification.Message =
                "Your request to join '" +
                request.Idea.Title +
                "' has been accepted.";

            notification.IsRead = false;

            notification.CreatedAt =
                DateTime.Now;

            db.Notifications.Add(notification);

            db.SaveChanges();

            TempData["Success"] =
                "Join request accepted.";

            return RedirectToAction(
                "JoinRequests");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectRequest(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int ownerId =
                (int)Session["UserId"];

            var request = db.JoinRequests
                .Include("Idea")
                .FirstOrDefault(x =>
                    x.Id == id &&
                    x.Idea.UserId == ownerId);

            if (request == null)
                return HttpNotFound();

            request.Status =
                "Rejected";

            Notification notification =
                new Notification();

            notification.UserId =
                request.UserId;

            notification.Message =
                "Your request to join '" +
                request.Idea.Title +
                "' has been rejected.";

            notification.IsRead = false;

            notification.CreatedAt =
                DateTime.Now;

            db.Notifications.Add(notification);

            db.SaveChanges();

            TempData["Success"] =
                "Join request rejected.";

            return RedirectToAction(
                "JoinRequests");
        }
    }
}
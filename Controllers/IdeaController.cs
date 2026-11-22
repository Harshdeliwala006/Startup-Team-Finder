using System;
using System.Linq;
using System.Web.Mvc;
using demostartupfinder.Models;

namespace demostartupfinder.Controllers
{
    public class IdeaController : Controller
    {
        private ApplicationDbContext db =
            new ApplicationDbContext();

        public ActionResult BrowseIdeas(string search)
        {
            var ideas = db.Ideas
                .Include("User")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                ideas = ideas.Where(x =>
                    x.Title.Contains(search) ||
                    x.Description.Contains(search) ||
                    x.Category.Contains(search) ||
                    x.RequiredSkills.Contains(search));
            }

            ViewBag.Search = search;

            return View(
                ideas
                .OrderByDescending(x => x.CreatedAt)
                .ToList()
            );
        }

        public ActionResult CreateIdea()
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateIdea(Idea idea)
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            if (ModelState.IsValid)
            {
                idea.UserId =
                    (int)Session["UserId"];

                idea.CreatedAt =
                    DateTime.Now;

                db.Ideas.Add(idea);

                db.SaveChanges();

                TempData["Success"] =
                    "Startup idea created successfully.";

                return RedirectToAction(
                    "MyIdeas");
            }

            return View(idea);
        }

        public ActionResult MyIdeas()
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int userId =
                (int)Session["UserId"];

            var ideas = db.Ideas
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return View(ideas);
        }

        public ActionResult Details(int id)
        {
            var idea = db.Ideas
                .Include("User")
                .FirstOrDefault(x => x.Id == id);

            if (idea == null)
                return HttpNotFound();

            return View(idea);
        }


        public ActionResult Edit(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int userId =
                (int)Session["UserId"];

            var idea = db.Ideas.FirstOrDefault(x =>
                x.Id == id &&
                x.UserId == userId);

            if (idea == null)
                return HttpNotFound();

            return View(idea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Idea idea)
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int userId =
                (int)Session["UserId"];

            var existingIdea = db.Ideas.FirstOrDefault(x =>
                x.Id == idea.Id &&
                x.UserId == userId);

            if (existingIdea == null)
                return HttpNotFound();

            if (ModelState.IsValid)
            {
                existingIdea.Title =
                    idea.Title;

                existingIdea.Description =
                    idea.Description;

                existingIdea.Category =
                    idea.Category;

                existingIdea.RequiredSkills =
                    idea.RequiredSkills;

                existingIdea.TeamSize =
                    idea.TeamSize;

                db.SaveChanges();

                TempData["Success"] =
                    "Startup idea updated successfully.";

                return RedirectToAction(
                    "MyIdeas");
            }

            return View(idea);
        }

 
        public ActionResult Delete(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int userId =
                (int)Session["UserId"];

            var idea = db.Ideas.FirstOrDefault(x =>
                x.Id == id &&
                x.UserId == userId);

            if (idea != null)
            {
                var savedIdeas = db.SavedIdeas
                    .Where(x => x.IdeaId == id)
                    .ToList();

                db.SavedIdeas.RemoveRange(savedIdeas);

                var requests = db.JoinRequests
                    .Where(x => x.IdeaId == id)
                    .ToList();

                db.JoinRequests.RemoveRange(requests);

                db.Ideas.Remove(idea);

                db.SaveChanges();
            }

            return RedirectToAction(
                "MyIdeas");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveIdea(int ideaId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int userId =
                (int)Session["UserId"];

            bool alreadySaved =
                db.SavedIdeas.Any(x =>
                    x.UserId == userId &&
                    x.IdeaId == ideaId);

            if (!alreadySaved)
            {
                SavedIdea savedIdea =
                    new SavedIdea();

                savedIdea.UserId = userId;
                savedIdea.IdeaId = ideaId;

                db.SavedIdeas.Add(savedIdea);

                db.SaveChanges();

                TempData["Success"] =
                    "Idea saved successfully.";
            }

            return RedirectToAction(
                "Details",
                new { id = ideaId });
        }
        public ActionResult RemoveSavedIdea(int ideaId)
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int userId =
                (int)Session["UserId"];

            var savedIdea =
                db.SavedIdeas.FirstOrDefault(x =>
                    x.UserId == userId &&
                    x.IdeaId == ideaId);

            if (savedIdea != null)
            {
                db.SavedIdeas.Remove(savedIdea);

                db.SaveChanges();
            }

            return RedirectToAction(
                "SavedIdeas");
        }

        public ActionResult SavedIdeas()
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int userId =
                (int)Session["UserId"];

            var ideas = db.SavedIdeas
                .Where(x => x.UserId == userId)
                .Select(x => x.Idea)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return View(ideas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JoinTeam(
            int ideaId,
            string message)
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int userId =
                (int)Session["UserId"];

            var idea = db.Ideas.Find(ideaId);

            if (idea == null)
                return HttpNotFound();

            // User cannot join own idea
            if (idea.UserId == userId)
            {
                TempData["Error"] =
                    "You cannot join your own startup idea.";

                return RedirectToAction(
                    "Details",
                    new { id = ideaId });
            }

            bool alreadyRequested =
                db.JoinRequests.Any(x =>
                    x.IdeaId == ideaId &&
                    x.UserId == userId &&
                    x.Status == "Pending");

            if (alreadyRequested)
            {
                TempData["Error"] =
                    "You already sent a request for this idea.";

                return RedirectToAction(
                    "Details",
                    new { id = ideaId });
            }

            JoinRequest request =
                new JoinRequest();

            request.IdeaId = ideaId;
            request.UserId = userId;
            request.Message = message;
            request.Status = "Pending";
            request.RequestDate = DateTime.Now;

            db.JoinRequests.Add(request);

            // Create notification for idea owner
            Notification notification =
                new Notification();

            notification.UserId =
                idea.UserId;

            notification.Message =
                Session["UserName"] +
                " wants to join your startup idea: " +
                idea.Title;

            notification.IsRead = false;

            notification.CreatedAt =
                DateTime.Now;

            db.Notifications.Add(notification);

            db.SaveChanges();

            TempData["Success"] =
                "Join request sent successfully.";

            return RedirectToAction(
                "Details",
                new { id = ideaId });
        }
    }
}
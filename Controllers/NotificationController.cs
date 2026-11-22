using System.Linq;
using System.Web.Mvc;
using demostartupfinder.Models;

namespace demostartupfinder.Controllers
{
    public class NotificationController : Controller
    {
        private ApplicationDbContext db =
            new ApplicationDbContext();
        public ActionResult Notifications()
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int userId =
                (int)Session["UserId"];

            var notifications =
                db.Notifications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            // Mark notifications as read
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            db.SaveChanges();

            return View(notifications);
        }

        public ActionResult Delete(int id)
        {
            if (Session["UserId"] == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int userId =
                (int)Session["UserId"];

            var notification =
                db.Notifications.FirstOrDefault(x =>
                    x.Id == id &&
                    x.UserId == userId);

            if (notification != null)
            {
                db.Notifications.Remove(notification);
                db.SaveChanges();
            }

            return RedirectToAction(
                "Notifications");
        }
    }
}
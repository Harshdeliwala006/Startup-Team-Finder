using System;
using System.Linq;
using System.Web.Mvc;
using demostartupfinder.Models;

namespace demostartupfinder.Controllers
{
    public class MessageController : Controller
    {
        private ApplicationDbContext db =
            new ApplicationDbContext();

        public ActionResult Messages(int? userId)
        {
            // Check login
            if (Session["UserId"] == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            int currentUserId =
                (int)Session["UserId"];

            // Get all users except current user
            var users = db.Users
                .Where(x => x.Id != currentUserId)
                .OrderBy(x => x.Name)
                .ToList();

            ViewBag.Users = users;

            // If a user is selected
            if (userId.HasValue)
            {
                // Find selected user
                var selectedUser =
                    db.Users.Find(userId.Value);

                if (selectedUser == null)
                {
                    return HttpNotFound();
                }

                ViewBag.SelectedUser =
                    selectedUser;

                // Get conversation between
                // current user and selected user
                var messages = db.Messages
                    .Where(x =>
                        (x.SenderId == currentUserId &&
                         x.ReceiverId == userId.Value)
                        ||
                        (x.SenderId == userId.Value &&
                         x.ReceiverId == currentUserId))
                    .OrderBy(x => x.SentAt)
                    .ToList();

                var unreadMessages = db.Messages
                    .Where(x =>
                        x.SenderId == userId.Value &&
                        x.ReceiverId == currentUserId &&
                        !x.IsRead)
                    .ToList();

                foreach (var message in unreadMessages)
                {
                    message.IsRead = true;
                }

                db.SaveChanges();

                return View(messages);
            }

            // No user selected
            return View(
                new System.Collections.Generic.List<Message>()
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Send(
            int receiverId,
            string text)
        {
            // Check login
            if (Session["UserId"] == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            int senderId =
                (int)Session["UserId"];

            if (senderId == receiverId)
            {
                TempData["Error"] =
                    "You cannot send a message to yourself.";

                return RedirectToAction(
                    "Messages",
                    new { userId = receiverId });
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                TempData["Error"] =
                    "Message cannot be empty.";

                return RedirectToAction(
                    "Messages",
                    new { userId = receiverId });
            }

            var receiver =
                db.Users.Find(receiverId);

            if (receiver == null)
            {
                return HttpNotFound();
            }

            Message message =
                new Message();

            message.SenderId =
                senderId;

            message.ReceiverId =
                receiverId;

            message.Text =
                text.Trim();

            message.SentAt =
                DateTime.Now;

            message.IsRead =
                false;


            // Add message to database
            db.Messages.Add(message);

            Notification notification =
                new Notification();

            // Notification belongs to receiver
            notification.UserId =
                receiverId;

            // Notification message
            notification.Message =
                Session["UserName"] +
                " sent you a new message.";

            // New notification is unread
            notification.IsRead =
                false;

            // Notification creation time
            notification.CreatedAt =
                DateTime.Now;


            // Add notification to database
            db.Notifications.Add(notification);

            db.SaveChanges();

            return RedirectToAction(
                "Messages",
                new { userId = receiverId });
        }
    }
}
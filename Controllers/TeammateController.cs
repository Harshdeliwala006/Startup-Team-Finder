using System.Linq;
using System.Web.Mvc;
using demostartupfinder.Models;

namespace demostartupfinder.Controllers
{
    [Authorize]
    public class TeammateController : Controller
    {
        private ApplicationDbContext db =
            new ApplicationDbContext();

        // GET: Teammate/Find
        public ActionResult Find(string search)
        {
            var users = db.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                users = users.Where(x =>
                    x.Name.Contains(search) ||
                    x.Bio.Contains(search) ||
                    x.Location.Contains(search) ||
                    db.Skills.Any(s =>
                        s.UserId == x.Id &&
                        s.SkillName.Contains(search))
                );
            }

            ViewBag.Search = search;
            return View(users.ToList());
        }

        // GET: Teammate/Details/5
        public ActionResult Details(int id)
        {
            var user = db.Users
                .Include("Skills")
                .FirstOrDefault(x => x.Id == id);

            if (user == null)
                return HttpNotFound();

            return View(user);
        }
    }
}

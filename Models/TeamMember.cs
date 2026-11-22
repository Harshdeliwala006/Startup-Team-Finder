using demostartupfinder.Models;

namespace demostartupfinder.Models
{
    public class TeamMember
    {
        public int Id { get; set; }

        public int TeamId { get; set; }

        public int UserId { get; set; }

        public string Position { get; set; }

        public virtual Team Team { get; set; }

        public virtual User User { get; set; }
    }
}
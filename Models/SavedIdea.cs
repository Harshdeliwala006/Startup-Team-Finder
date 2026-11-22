using demostartupfinder.Models;

namespace demostartupfinder.Models
{
    public class SavedIdea
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int IdeaId { get; set; }

        public virtual Idea Idea { get; set; }
    }
}
namespace demostartupfinder.Models
{
    public class Team
    {
        public int Id { get; set; }

        public string TeamName { get; set; }

        public string Description { get; set; }

        public int IdeaId { get; set; }

        public virtual Idea Idea { get; set; }

        public int OwnerId { get; set; }
    }
}
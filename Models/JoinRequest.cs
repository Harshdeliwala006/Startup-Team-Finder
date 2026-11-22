using System;

namespace demostartupfinder.Models
{
    public class JoinRequest
    {
        public int Id { get; set; }

        public int IdeaId { get; set; }

        public virtual Idea Idea { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public string Message { get; set; }

        public string Status { get; set; }

        public DateTime RequestDate { get; set; }
    }
}
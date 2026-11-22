using System;
using System.ComponentModel.DataAnnotations;

namespace demostartupfinder.Models
{
    public class Idea
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string Category { get; set; }

        public string RequiredSkills { get; set; }

        public int TeamSize { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
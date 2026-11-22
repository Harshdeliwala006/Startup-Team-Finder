
using System.ComponentModel.DataAnnotations;

namespace demostartupfinder.Models
{
    public class Skill
    {
        public int Id { get; set; }

        [Required]
        public string SkillName { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}

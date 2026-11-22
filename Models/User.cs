using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace demostartupfinder.Models
{
    public class User
    {
        public User()
        {
            Skills = new HashSet<Skill>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Bio { get; set; }
        public string ProfileImage { get; set; }
        public string Location { get; set; }
        public string Role { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Skill> Skills { get; set; }
    }
}
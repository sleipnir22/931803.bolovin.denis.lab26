using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class ForumCategory
    {
        public Guid Id { get; set; } 

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        
        public Guid AuthorId { get; set; }
        public User Author { get; set; }
        
        public ICollection<Post> Posts { get; set; }
    }
}
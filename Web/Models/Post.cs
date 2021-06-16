using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        
        public Guid AuthorId { get; set; }
        
        public User Author { get; set; }
        
        public Guid CategoryId { get; set; }

        public ForumCategory Category { get; set; }
        
        public ICollection<Attachment> Attachments { get; set; }
        
        [Required]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
        
        public DateTime CreateTime { get; set; }
        
        public DateTime EditTime { get; set; }
        
        public ICollection<Comment> Comments { get; set; }
    }
}
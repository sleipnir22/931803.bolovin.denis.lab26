using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        
        public Guid AuthorId { get; set; }
        public User Author { get; set; }
        
        public Guid PostId { get; set; }
        public Post Post { get; set; }
        
        public DateTime Time { get; set; }
        
        [Required]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
    }
}
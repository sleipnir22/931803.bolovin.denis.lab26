using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Attachment
    {
        public Guid Id { get; set; }
        
        public Guid PostId { get; set; }
        
        public Post Post { get; set; }
        
        [Url]
        [Required]
        public string Url { get; set; }

        public bool IsImage { get; set; }
        
        public string Alt { get; set; }
    }
}
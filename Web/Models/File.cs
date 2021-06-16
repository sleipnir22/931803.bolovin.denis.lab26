using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class File
    {
        public Guid Id { get; set; }

        public Guid FolderId { get; set; }

        public Folder Folder { get; set; }
        
        public Guid OwnerId { get; set; }
        
        public User Owner { get; set; }

        public byte[] Body { get; set; }

        [Required]
        public string Name { get; set; }

        public int Size { get; set; }
    }
}
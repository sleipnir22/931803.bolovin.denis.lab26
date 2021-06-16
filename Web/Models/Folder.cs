using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Folder
    {
        public Guid Id { get; set; }
        
        public Guid? ParentFolderId { get; set; }
        
        public Folder ParentFolder { get; set; }

        public ICollection<File> Files { get; set; }
        
        public ICollection<Folder> Folders { get; set; }
        
        public Guid OwnerId { get; set; }
        
        public User Owner { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
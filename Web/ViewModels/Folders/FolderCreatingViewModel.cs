using System;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels.Folders
{
    public class FolderCreatingViewModel
    {
        [Required]
        public string Name { get; set; }
        
        public Guid ParentFolderId { get; set; } 
    }
}
using System;
using System.Collections.Generic;
using Web.Models;

namespace Web.ViewModels.Folders
{
    public class FolderViewModel
    {
        public Folder Folder { get; set; }

        public List<(string name, Guid id)> Path { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Models;
using Web.ViewModels.Folders;
using File = Web.Models.File;

namespace Web.Controllers
{
    public class FoldersController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public FoldersController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public IActionResult Index()
        {
            var folder = _dbContext.Folders
                .Include(x=>x.Owner)
                .Include(x => x.Files).ThenInclude(x=>x.Owner)
                .Include(x => x.Folders).ThenInclude(x=>x.Owner)
                .First(x => x.ParentFolderId == null);
            
            return View(new FolderViewModel
            {
                Folder = folder,
                Path = GetPath(folder)
            });
        }

        [HttpGet("[controller]/{id:guid}")]
        public IActionResult GetFolder(Guid id)
        {
            if (ModelState.IsValid)
            {
                var folder = _dbContext.Folders
                    .Include(x=>x.Owner)
                    .Include(x=>x.Files).ThenInclude(x=>x.Owner)
                    .Include(x=>x.Folders).ThenInclude(x=>x.Owner)
                    .Include(x=>x.ParentFolder).ThenInclude(x=>x.Owner)
                    .FirstOrDefault(x => x.Id == id);
                
                if (folder is null)
                    return NotFound();

                return View("Index", new FolderViewModel
                {
                    Folder = folder,
                    Path = GetPath(folder)
                });
            }

            return BadRequest();
        }
        
        [HttpGet("[controller]/{ParentFolderId:guid}/Create")]
        [Authorize]
        public IActionResult Create(Guid parentFolderId)
        {
            if (ModelState.IsValid)
            {
                var parentFolder = _dbContext.Folders.FirstOrDefault(x => x.Id == parentFolderId);
                if (parentFolder is null)
                    return NotFound();

                return View(new FolderCreatingViewModel {ParentFolderId = parentFolderId});
            }

            return BadRequest();
        }
        
        [HttpPost("[controller]/{ParentFolderId:guid}/Create")]
        [Authorize]
        public IActionResult Create(FolderCreatingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var parentFolder = _dbContext.Folders.FirstOrDefault(x => x.Id == model.ParentFolderId);
                if (parentFolder is null)
                    return NotFound();

                var folder = new Folder
                {
                    Name = model.Name,
                    OwnerId = new Guid(HttpContext.User.FindFirstValue("Id")),
                    ParentFolderId = parentFolder.Id
                };

                _dbContext.Add(folder);
                _dbContext.SaveChanges();

                return Redirect($"/Folders/{parentFolder.Id}");
            }

            return View(model);
        }

        [HttpGet("[controller]/{id:guid}/Delete")]
        [Authorize]
        public IActionResult Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                var folder = _dbContext.Folders.FirstOrDefault(x => x.Id == id);
                if (folder is null)
                    return NotFound();

                var userId = new Guid(HttpContext.User.FindFirstValue("Id"));
                if (folder.OwnerId != userId && HttpContext.User.IsInRole("Admin"))
                    return Unauthorized();

                _dbContext.Remove(folder);
                _dbContext.SaveChanges();

                return Redirect($"/Folders/{folder.ParentFolderId}");
            }

            return BadRequest();
        }

        [HttpGet("[controller]/{id:guid}/Upload")]
        [Authorize]
        public IActionResult Upload(Guid id)
        {
            if (ModelState.IsValid)
            {
                var folder = _dbContext.Folders.FirstOrDefault(x => x.Id == id);
                if (folder is null)
                    return NotFound();

                ViewBag.FolderId = id;
                return View();
            }

            return BadRequest();
        }
        
        [HttpPost("[controller]/{folderId:guid}/Upload")]
        [Authorize]
        public IActionResult Upload(Guid folderId,[FromForm] [Required] IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var folder = _dbContext.Folders.FirstOrDefault(x => x.Id == folderId);
                if (folder is null)
                    return NotFound();
                
                var stream = file.OpenReadStream();
                using var reader = new BinaryReader(stream);
                _dbContext.Files.Add(new File
                {
                    Body = reader.ReadBytes((int) file.Length),
                    Name = file.FileName,
                    FolderId = folderId,
                    OwnerId = new Guid(HttpContext.User.FindFirstValue("Id")),
                    Size = (int) file.Length
                });
                _dbContext.SaveChanges();

                return Redirect($"/Folders/{folderId}");
            }

            ViewBag.FolderId = folderId;
            return View();
        }

        private List<(string name, Guid id)> GetPath(Folder folder)
        {
            var stack = new Stack<(string name, Guid id)>();
            stack.Push((folder.Name, folder.Id));
            while (folder.ParentFolderId != null)
            {
                folder = _dbContext.Folders.First(x => x.Id == folder.ParentFolderId);
                stack.Push((folder.Name, folder.Id));
            }
            
            return stack.ToList();
        }
    }
}
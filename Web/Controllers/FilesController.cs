using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Data;

namespace Web.Controllers
{
    public class FilesController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public FilesController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("[controller]/{id:guid}")]
        public IActionResult GetFile(Guid id)
        {
            if (ModelState.IsValid)
            {
                var file = _dbContext.Files
                    .Include(x=>x.Folder)
                    .FirstOrDefault(x => x.Id == id);
                
                if (file is null)
                    return NotFound();

                return File(file.Body,"application/octet-stream", file.Name);
            }

            return BadRequest();
        }
        
        [HttpGet("[controller]/{id:guid}/Delete")]
        [Authorize]
        public IActionResult Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                var file = _dbContext.Files.FirstOrDefault(x => x.Id == id);
                if (file is null)
                    return NotFound();

                var userId = new Guid(HttpContext.User.FindFirstValue("Id"));
                if (file.OwnerId != userId && HttpContext.User.IsInRole("Admin"))
                    return Unauthorized();

                _dbContext.Remove(file);
                _dbContext.SaveChanges();

                return Redirect($"/Folders/{file.FolderId}");
            }

            return BadRequest();
        }
    }
}
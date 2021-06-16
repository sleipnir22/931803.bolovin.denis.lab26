using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Models;

namespace Web.Controllers
{
    public class ForumCategoriesController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ForumCategoriesController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET
        public IActionResult Index()
        {
            var categories = _dbContext.ForumCategories
                .Include(x => x.Posts)
                .Select(x => new CategoriesViewModel(x.Id, x.Name, x.Description, x.Author, x.Posts.Count))
                .ToList();
            
            return View(categories);
        }
        public record CategoriesViewModel(Guid Id, string Name, string Description, User Author, int PostCount);

        [HttpGet("[controller]/{id:guid}")]
        public IActionResult GetCategory(Guid id)
        {
            if (ModelState.IsValid)
            {
                var category = _dbContext.ForumCategories
                    .Include(x=>x.Author)
                    .Include(x=> x.Posts)
                    .ThenInclude(x=>x.Comments)
                    .ThenInclude(x=>x.Author)
                    .FirstOrDefault(x => x.Id == id);

                if (category is null)
                    return NotFound();

                return View("Category", category);
            }

            return BadRequest();
        }

        [Authorize] [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        [Authorize] [HttpPost]
        public IActionResult Create(CategoryCreatingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = new ForumCategory
                {
                    Name = model.Name,
                    Description = model.Description,
                    AuthorId = new Guid(HttpContext.User.FindFirstValue("Id"))
                };

                _dbContext.Add(category);
                _dbContext.SaveChanges();

                return Redirect($"/ForumCategories/{category.Id}");
            }

            return View(model);
        }

        public record CategoryCreatingViewModel(
            [Required] string Name,
            [DataType(DataType.MultilineText)] [Required] string Description);
        
        [Authorize] [HttpGet("[controller]/{id:guid}/Delete")]
        public IActionResult Delete(Guid id)
        {
            if (ModelState.IsValid)
            {
                var category = _dbContext.ForumCategories
                    .FirstOrDefault(x => x.Id == id);
                if (category is null)
                    return NotFound();

                var userId = new Guid(HttpContext.User.FindFirstValue("Id"));
                if (category.AuthorId != userId && HttpContext.User.IsInRole("Admin"))
                    return Unauthorized();

                _dbContext.Remove(category);
                _dbContext.SaveChanges();

                return Redirect($"/ForumCategories");
            }

            return BadRequest();
        }

        [Authorize] [HttpGet("[controller]/{id:guid}/Edit")]
        public IActionResult Edit(Guid id)
        {
            if (ModelState.IsValid)
            {
                var category = _dbContext.ForumCategories
                    .FirstOrDefault(x => x.Id == id);
                if (category is null)
                    return NotFound();

                var userId = new Guid(HttpContext.User.FindFirstValue("Id"));
                if (category.AuthorId != userId && !HttpContext.User.IsInRole("Admin"))
                    return Unauthorized();

                return View(new CategoryEditingViewModel
                {
                    Name = category.Name,
                    Description = category.Description,
                    Id = category.Id
                });
            }
            
            return BadRequest();
        }
        
        [Authorize] [HttpPost("[controller]/{id:guid}/Edit")]
        public IActionResult Edit(CategoryEditingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = _dbContext.ForumCategories
                    .FirstOrDefault(x => x.Id == model.Id);
                if (category is null)
                    return NotFound();

                var userId = new Guid(HttpContext.User.FindFirstValue("Id"));
                if (category.AuthorId != userId && !HttpContext.User.IsInRole("Admin"))
                    return Unauthorized();

                category.Name = model.Name;
                category.Description = model.Description;
                _dbContext.Update(category);
                _dbContext.SaveChanges();

                return Redirect($"/ForumCategories");
            }

            return View(model);
        }
        public record CategoryEditingViewModel
        {
            public string Name { get; init; }
            public string Description { get; init; }
            [FromRoute]
            public Guid Id { get; init; }
        }
    }
}
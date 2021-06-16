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
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public PostsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("[controller]/{id:guid}")]
        public IActionResult GetPost(Guid id)
        {
            if (!ModelState.IsValid) return BadRequest();
            
            var post = _dbContext.Posts
                .Include(x=>x.Author)
                .Include(x=>x.Attachments)
                .Include(x=>x.Category)
                .Include(x=>x.Comments)
                .ThenInclude(x=>x.Author)
                .FirstOrDefault(x => x.Id == id);
                
            if (post is null)
                return NotFound();
                
            return View("Post", post);

        }

        [HttpGet]
        [Authorize]
        public IActionResult Create(Guid categoryId)
        {
            if (!ModelState.IsValid) return BadRequest();

            var categories = _dbContext.ForumCategories.ToList();
            if (categories.FirstOrDefault(x => x.Id == categoryId) is null)
                return NotFound();

            return View(new PostCreatingViewModel
            {
                CategoryId = categoryId,
                Categories = categories
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(PostCreatingViewModel model)
        {
            var categories = _dbContext.ForumCategories.ToList();
            model.Categories = categories;
            if (!ModelState.IsValid) return View(model);

            var post = new Post
            {
                Title = model.Title,
                Text = model.Text,
                AuthorId = new Guid(HttpContext.User.FindFirstValue("Id")),
                CategoryId = model.CategoryId,
                CreateTime = DateTime.Now,
                EditTime = DateTime.Now
            };
            _dbContext.Add(post);
            _dbContext.SaveChanges();

            return Redirect($"/Posts/{post.Id}");
        }

        [Authorize]
        [HttpGet("[controller]/{id:guid}/Delete")]
        public IActionResult Delete(Guid id)
        {
            if (ModelState.IsValid is false) return BadRequest();
            
            var post = _dbContext.Posts.FirstOrDefault(x => x.Id == id);
            if (post is null) return NotFound();

            _dbContext.Remove(post);
            _dbContext.SaveChanges();

            return Redirect($"/ForumCategories/{post.CategoryId}");
        }

        [Authorize]
        [HttpGet("[controller]/{postId:guid}/Comments/Create")]
        public IActionResult CreateComment(Guid postId)
        {
            if (ModelState.IsValid is false) return BadRequest();

            return View(new CommentCreatingViewModel{PostId = postId});
        }
        
        [Authorize]
        [HttpPost("[controller]/{postId:guid}/Comments/Create")]
        public IActionResult CreateComment(CommentCreatingViewModel model)
        {
            if (ModelState.IsValid is false) return View(model);

            var comment = new Comment
            {
                PostId = model.PostId,
                Text = model.Text,
                AuthorId = new Guid(HttpContext.User.FindFirstValue("Id")),
                Time = DateTime.Now
            };

            _dbContext.Add(comment);
            _dbContext.SaveChanges();

            return Redirect($"/Posts/{comment.PostId}");
        }
        public class CommentCreatingViewModel
        {
            public Guid PostId { get; set; }
            [Required]
            public string Text { get; set; }
        }

        [Authorize]
        [HttpGet("[controller]/{postId:guid}/Comments/{id:guid}/Delete")]
        public IActionResult DeleteComment(Guid id)
        {
            var comment = _dbContext.Comments.FirstOrDefault(x => x.Id == id);
            _dbContext.Remove(comment);
            _dbContext.SaveChanges();
            return Redirect($"/Posts/{comment.PostId}");
        }

        [Authorize]
        [HttpGet("[controller]/{postId:guid}/Attach")]
        public IActionResult Attach(Guid postId)
        {
            return View(new AttachingViewModel
            {
                PostId = postId
            });
        }
        
        [Authorize]
        [HttpPost("[controller]/{postId:guid}/Attach")]
        public IActionResult Attach(AttachingViewModel model)
        {
            if (ModelState.IsValid is false) return View(model);

            var attachment = new Attachment
            {
                PostId = model.PostId,
                Url = model.Url,
                IsImage = model.IsImage,
                Alt = model.Alt
            };

            _dbContext.Add(attachment);
            _dbContext.SaveChanges();
            return Redirect($"/Posts/{model.PostId}");
        }

        public class AttachingViewModel
        {
            public Guid PostId { get; set; }
            [Required] [DataType(DataType.Url)]
            public string Url { get; set; }
            public bool IsImage { get; set; }
            public string Alt { get; set; }
        }

        [Authorize]
        [HttpGet("[controller]/{postId:guid}/Deattach")]
        public IActionResult Deattach(Guid id)
        {
            var attach = _dbContext.Attachments.FirstOrDefault(x => x.Id == id);
            _dbContext.Remove(attach);
            _dbContext.SaveChanges();
            return Redirect($"/Posts/{attach.PostId}");
        }

        public class PostCreatingViewModel
        {
            public Guid CategoryId { get; set; }
            [Required]
            public string Title { get; set; }
            [Required]
            public string Text { get; set; }
            
            public IEnumerable<ForumCategory> Categories { get; set; }
        }
    }
}
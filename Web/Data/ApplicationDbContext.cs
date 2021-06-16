using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<Attachment> Attachments { get; set; }
        
        public DbSet<File> Files { get; set; }
        
        public DbSet<Folder> Folders { get; set; }
        
        public DbSet<ForumCategory> ForumCategories { get; set; }
        
        public DbSet<Post> Posts { get; set; }
        
        public DbSet<Comment> Comments { get; set; }
        
        public DbSet<User> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Folder>()
                .HasOne(x => x.ParentFolder)
                .WithMany(x => x.Folders)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<File>()
                .HasOne(x => x.Folder)
                .WithMany(x => x.Files)
                .OnDelete(DeleteBehavior.Cascade);

            #region Инициализация БД начальными сущностями
            {
                var admin = new User
                {
                    Id = Guid.NewGuid(),
                    IsAdmin = true,
                    Email = "admin@localhost",
                    Password = "SuperPassword",
                };

                var offtopCategory = new ForumCategory
                {
                    Id = Guid.NewGuid(),
                    AuthorId = admin.Id,
                    Name = "Offtop",
                    Description = "About everything and nothing"
                };

                var firstOfftop = new Post
                {
                    Id = Guid.NewGuid(),
                    CategoryId = offtopCategory.Id,
                    AuthorId = admin.Id,
                    Title = "First offtop",
                    Text = "First offtop body",
                    CreateTime = DateTime.Now,
                    EditTime = DateTime.Now
                };

                var comments = new List<Comment>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        AuthorId = admin.Id,
                        Text = "First comment",
                        Time = DateTime.Now,
                        PostId = firstOfftop.Id
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        AuthorId = admin.Id,
                        Text = "Second comment",
                        Time = DateTime.Now,
                        PostId = firstOfftop.Id
                    }
                };

                var secondOfftop = new Post
                {
                    Id = Guid.NewGuid(),
                    CategoryId = offtopCategory.Id,
                    AuthorId = admin.Id,
                    Title = "Second offtop",
                    Text = "Second offtop body",
                    CreateTime = DateTime.Now,
                    EditTime = DateTime.Now,
                };
                

                var attachments = new List<Attachment>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        PostId = firstOfftop.Id,
                        Alt = "Cat",
                        IsImage = true,
                        Url =
                            "https://sun9-3.userapi.com/impg/BPUeejEopoM_0FCvrbgD7gsUpjVIuHgjqLoHow/9ACt_gLIUi8.jpg?size=1125x1096&quality=96&sign=29de9a9f5ba74558e1c1320d650eb297&type=album"
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        PostId = firstOfftop.Id,
                        Alt = "Repository",
                        Url = "https://github.com/RuDomitori/931801.zhavoronkov.dmitry.lab5"
                    }
                };

                var rootFolder = new Folder
                {
                    Id = Guid.NewGuid(),
                    Name = "Root",
                    OwnerId = admin.Id,
                };

                var folders = new Folder[]
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        ParentFolderId = rootFolder.Id,
                        Name = "Some folder",
                        OwnerId = admin.Id,
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        ParentFolderId = rootFolder.Id,
                        Name = "Some folder too",
                        OwnerId = admin.Id,
                    }
                };

                var files = new File[]
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        FolderId = rootFolder.Id,
                        Name = "Some file",
                        Body = new byte[] {123, 123, 12, 3, 123, 123, 123, 123},
                        Size = 8,
                        OwnerId = admin.Id,
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        FolderId = rootFolder.Id,
                        Name = "Some file too",
                        Body = new byte[] {1, 2, 3, 4, 5, 6, 7, 8},
                        Size = 8,
                        OwnerId = admin.Id,
                    }
                };

                builder.Entity<User>().HasData(admin);
                
                builder.Entity<ForumCategory>().HasData(offtopCategory);
                builder.Entity<Post>().HasData(firstOfftop, secondOfftop);
                builder.Entity<Comment>().HasData(comments);
                builder.Entity<Attachment>().HasData(attachments);
                
                builder.Entity<Folder>().HasData(rootFolder);
                builder.Entity<File>().HasData(files);
                builder.Entity<Folder>().HasData(folders);
            }
            #endregion
        }
    }
}
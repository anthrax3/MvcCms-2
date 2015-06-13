using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcCms.Models;
using System.Data.Entity;

namespace MvcCms.Data
{
    public class PostRepository : IPostRepository
    {
        public int CountPublished
        {
            get
            {
                using(var db = new CmsContext())
                {
                    return db.Posts.Count(p => p.Published < DateTime.Now);
                }
            }
        }

        public async Task<Post> GetAsync(string id)
        {
            using (var db = new CmsContext())
            {
                return await db.Posts.Include("Author")
                                     .SingleOrDefaultAsync(post => post.Id == id);
            }
        }

        public void Edit(string id, Post updatedItem)
        {
            using(var db = new CmsContext())
            {
                var post = db.Posts.SingleOrDefault(p => p.Id == id);

                if(post == null)
                {
                    throw new KeyNotFoundException("A post with the id of " + id + " does not exist.");
                }

                post.Id = updatedItem.Id;
                post.Title = updatedItem.Title;
                post.Content = updatedItem.Content;
                post.Published = updatedItem.Published;
                post.Tags = updatedItem.Tags;

                db.SaveChanges();
            }
        }

        public void Create(Post model)
        {
            using(var db = new CmsContext())
            {
                var post = db.Posts.SingleOrDefault(p => p.Id == model.Id);

                if(post != null)
                {
                    throw new ArgumentException("A post with id " + model.Id + " already exists.");
                }                
                
                db.Posts.Add(model);
                db.SaveChanges();
            }
        }

        public void Delete(string id)
        {
            using(var db = new CmsContext())
            {
                var post = db.Posts.SingleOrDefault(p => p.Id == id);
                
                if(post == null)
                {
                    throw new KeyNotFoundException("The post with the id " + id + " does not exist.");
                }

                db.Posts.Remove(post);
                db.SaveChanges();
            }
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            using (var db = new CmsContext())
            {
                return await db.Posts.Include("Author")
                                     .OrderByDescending(p => p.Created).ToArrayAsync();
            }
        }

        public async Task<IEnumerable<Post>> GetPostsByAuthorAsync(string authorId)
        {
            using (var db = new CmsContext())
            {
                return await db.Posts.Include("Author")
                                     .Where(p => p.AuthorId == authorId)
                                     .OrderByDescending(p => p.Created).ToArrayAsync();
            }
        }

        public async Task<IEnumerable<Post>> GetPublishedPostsAsync()
        {
            using(var db = new CmsContext())
            {
                return await db.Posts.Include("Author")
                                     .Where(p => p.Published < DateTime.Now)
                                     .OrderByDescending(p => p.Published)
                                     .ToArrayAsync();
            }
        }

        public async Task<IEnumerable<Post>> GetPostsByTagAsync(string tagId)
        {
            using(var db = new CmsContext())
            {
                var posts = await db.Posts.Include("Author")
                              .Where(p => p.CombinedTags.Contains(tagId))
                              .ToListAsync();

                return posts.Where(p =>
                            p.Tags.Contains(tagId, StringComparer.CurrentCultureIgnoreCase))
                                  .ToList();
            }
        }

        public async Task<IEnumerable<Post>> GetPageAsync(int pageNumber, int pageSize)
        {
            using(var db = new CmsContext())
            {
                var skip = (pageNumber - 1) * pageSize;

                return await db.Posts
                         .Where(p => p.Published < DateTime.Now)
                         .Include("Author")
                         .OrderByDescending(p => p.Published)
                         .Skip(skip)
                         .Take(pageSize)
                         .ToArrayAsync();
            }
        }
    }
}

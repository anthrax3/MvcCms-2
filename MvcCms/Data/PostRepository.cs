using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcCms.Models;
using System.Data.Entity;
using System.Linq.Expressions;

namespace MvcCms.Data
{
    public class PostRepository : IPostRepository
    {
        private readonly CmsContext _context;

        public PostRepository(CmsContext context)
        {
            _context = context;
        }

        public PostRepository()
            : this(new CmsContext())
        {
        }

        public int CountPublished
        {
            get
            {
                return _context.Posts.Count(p => p.Published < DateTime.Now);
            }
        }

        public async Task<Post> GetAsync(string id)
        {
            return await _context.Posts.Include("Author")
                                 .SingleOrDefaultAsync(post => post.Id == id);
        }

        public void Edit(string id, Post updatedItem)
        {
            var post = _context.Posts.SingleOrDefault(p => p.Id == id);

            if (post == null)
            {
                throw new KeyNotFoundException("A post with the id of " + id + " does not exist.");
            }

            post.Id = updatedItem.Id;
            post.Title = updatedItem.Title;
            post.Content = updatedItem.Content;
            post.Published = updatedItem.Published;
            post.Tags = updatedItem.Tags;

            _context.SaveChanges();

        }

        public void Create(Post model)
        {
            var post = _context.Posts.SingleOrDefault(p => p.Id == model.Id);

            if (post != null)
            {
                throw new ArgumentException("A post with id " + model.Id + " already exists.");
            }

            _context.Posts.Add(model);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var post = _context.Posts.SingleOrDefault(p => p.Id == id);

            if (post == null)
            {
                throw new KeyNotFoundException("The post with the id " + id + " does not exist.");
            }

            _context.Posts.Remove(post);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _context.Posts.Include("Author")
                                 .OrderByDescending(p => p.Created)
                                 .ToArrayAsync();
        }

        public async Task<IEnumerable<Post>> GetAllAsync(Expression<Func<Post, bool>> predicate)
        {
            return await _context.Posts.Include("Author")
                                 .Where(predicate)
                                 .OrderByDescending(p => p.Created)
                                 .ToArrayAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsByAuthorAsync(string authorId)
        {
            return await _context.Posts.Include("Author")
                                 .Where(p => p.AuthorId == authorId)
                                 .OrderByDescending(p => p.Created)
                                 .ToArrayAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsByAuthorAsync(string authorId, Expression<Func<Post, bool>> predicate)
        {
            return await _context.Posts.Include("Author")
                                 .Where(p => p.AuthorId == authorId)
                                 .Where(predicate)
                                 .OrderByDescending(p => p.Created)
                                 .ToArrayAsync();
        }

        public async Task<IEnumerable<Post>> GetPublishedPostsAsync()
        {
            return await _context.Posts.Include("Author")
                                 .Where(p => p.Published < DateTime.Now)
                                 .OrderByDescending(p => p.Published)
                                 .ToArrayAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsByTagAsync(string tagId)
        {
            var posts = await _context.Posts.Include("Author")
                          .Where(p => p.CombinedTags.Contains(tagId))
                          .ToListAsync();

            return posts.Where(p =>
                        p.Tags.Contains(tagId, StringComparer.CurrentCultureIgnoreCase))
                              .ToList();
        }

        public async Task<IEnumerable<Post>> GetPageAsync(int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1) * pageSize;

            return await _context.Posts
                     .Where(p => p.Published < DateTime.Now)
                     .Include("Author")
                     .OrderByDescending(p => p.Published)
                     .Skip(skip)
                     .Take(pageSize)
                     .ToArrayAsync();

        }        

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

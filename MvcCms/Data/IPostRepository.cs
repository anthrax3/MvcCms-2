using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcCms.Models;

namespace MvcCms.Data
{
    public interface IPostRepository
    {
        int CountPublished { get; }
        Task<Post> GetAsync(string id);
        void Edit(string id, Post updatedItem);
        void Create(Post model);
        void Delete(string id);
        Task<IEnumerable<Post>> GetAllAsync();
        Task<IEnumerable<Post>> GetPostsByAuthorAsync(string authorId);
        Task<IEnumerable<Post>> GetPublishedPostsAsync();
        Task<IEnumerable<Post>>  GetPostsByTagAsync(string tagId);
        Task<IEnumerable<Post>> GetPageAsync(int pageNumber, int pageSize);
    }
}

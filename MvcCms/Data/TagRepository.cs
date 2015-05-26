﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcCms.Data
{
    public class TagRepository : ITagRepository
    {
        public IEnumerable<string> GetAll()
        {
            using(var db = new CmsContext())
            {
                return db.Posts.SelectMany(p => p.Tags).Distinct();
            }
        }

        public string Get(string tag)
        {
            using(var db = new CmsContext())
            {
                var posts =
                    db.Posts.Where(p =>
                        p.Tags.Contains(tag, StringComparer.CurrentCultureIgnoreCase))
                      .ToList();

                if(!posts.Any())
                {
                    throw new KeyNotFoundException("The tag " + tag + " does not exist.");
                }

                return tag.ToLower();
            }
        }

        public void Edit(string existingTag, string newTag)
        {
            using(var db = new CmsContext())
            {
                var posts =
                    db.Posts.Where(p => 
                        p.Tags.Contains(existingTag, StringComparer.CurrentCultureIgnoreCase))
                        .ToList();

                if(!posts.Any())
                {
                    throw new KeyNotFoundException("The tag " + existingTag + " does not exist.");
                }

                foreach(var post in posts)
                {
                    post.Tags.Remove(existingTag);
                    post.Tags.Add(newTag);
                }

                db.SaveChanges();
            }
        }

        public void Delete(string tag)
        {
            using (var db = new CmsContext())
            {
                var posts =
                    db.Posts.Where(p =>
                        p.Tags.Contains(tag, StringComparer.CurrentCultureIgnoreCase))
                        .ToList();

                if (!posts.Any())
                {
                    throw new KeyNotFoundException("The tag " + tag + " does not exist.");
                }

                foreach (var post in posts)
                {
                    post.Tags.Remove(tag);                    
                }

                db.SaveChanges();
            }
        }
    }
}

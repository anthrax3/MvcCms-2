using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using MvcCms.Models;
using System.Data.Entity;

namespace MvcCms.Data
{
    public class CmsContext : IdentityDbContext<CmsUser>
    {
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Post>()
                        .HasKey(e => e.Id)
                        .Property(e => e.Id)
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Post>()
                        .HasRequired(e => e.Author);
        }
    }
}

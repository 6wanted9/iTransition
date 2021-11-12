using Couresework.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Couresework.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Review> Reviews { get; set; }
        
        public DbSet<ReviewStat> ReviewStats { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ReviewStat>()
                .HasKey(t => new { t.Id, t.ReviewId, t.UserId });

            modelBuilder.Entity<ReviewStat>()
                .HasOne(pt => pt.Review)
                .WithMany(p => p.ReviewStats)
                .HasForeignKey(pt => pt.ReviewId);

            modelBuilder.Entity<ReviewStat>()
                .HasOne(pt => pt.AspNetUsers)
                .WithMany(t => t.ReviewStats)
                .HasForeignKey(pt => pt.UserId);
        }
    }
}

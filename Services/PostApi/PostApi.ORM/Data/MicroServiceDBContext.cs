using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PostApi.ORM.Data
{
    public partial class MicroServiceDBContext : DbContext
    {
        public MicroServiceDBContext()
        {
        }

        public MicroServiceDBContext(DbContextOptions<MicroServiceDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Follow> Follows { get; set; }
        public virtual DbSet<PinnedPost> PinnedPosts { get; set; }
        public virtual DbSet<Post> Posts { get; set; } 
        public virtual DbSet<Rating> Ratings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {optionsBuilder.UseSqlServer("Server=DbAddress; Database=db_name; User Id=user_name; Password=userpassword; Integrated Security=False;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("aquafabu_microservice");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e=>e.Id);
                entity.Property(e => e.CategoryName).HasMaxLength(250);
            });

            modelBuilder.Entity<Follow>(entity =>
            {
                entity.ToTable("Follow");

                entity.Property(e => e.Id);
                entity.Property(e => e.FollowerId);
                entity.Property(e => e.FollowedId);
            });

            modelBuilder.Entity<PinnedPost>(entity =>
            {
                entity.ToTable("PinnedPost");

                entity.Property(e => e.Id);
                entity.Property(e => e.PostId);
                entity.Property(e => e.UserId);
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Post");

                entity.Property(e => e.Id);

                entity.Property(e => e.CategoryId);

                entity.Property(e => e.RatingCount);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Source).HasMaxLength(250);

                entity.Property(e => e.Thumbnail).HasMaxLength(250);
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.ToTable("Rating");

                entity.Property(e => e.Id);
                entity.Property(e => e.PostId);
                entity.Property(e => e.RatingCount);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

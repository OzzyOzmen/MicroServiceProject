using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebPage.ORM.Data
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

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Follow> Follows { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<PinnedPost> PinnedPosts { get; set; } = null!;
        public virtual DbSet<Post> Posts { get; set; } = null!;
        public virtual DbSet<Rating> Ratings { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=sql.zeus.domainhizmetleri.net;Database=aquafabu_microservice;user=aquafabu_microservice;Password=Microservice.3535;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("aquafabu_microservice");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryName).HasMaxLength(250);
            });

            modelBuilder.Entity<Follow>(entity =>
            {
                entity.ToTable("Follow");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.Notifications).HasMaxLength(250);
            });

            modelBuilder.Entity<PinnedPost>(entity =>
            {
                entity.ToTable("PinnedPost");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Post");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Source).HasMaxLength(250);

                entity.Property(e => e.Thumbnail).HasMaxLength(250);
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.ToTable("Rating");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NotificationApi.ORM.Data
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

      
        public virtual DbSet<Notification> Notifications { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {optionsBuilder.UseSqlServer("Server=sql.zeus.domainhizmetleri.net;Database=aquafabu_microservice;user=aquafabu_microservice;Password=Microservice.3535;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("aquafabu_microservice");

           
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.Id);
                entity.Property(e => e.Notifications).HasMaxLength(250);
                entity.Property(e=>e.FollowedUserId);
                entity.Property(e=>e.FollowerId);
                entity.Property(e=>e.PinnedVideosId);
                entity.Property(e => e.PostId);
                entity.Property(e=>e.RatingId);
            });

           

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

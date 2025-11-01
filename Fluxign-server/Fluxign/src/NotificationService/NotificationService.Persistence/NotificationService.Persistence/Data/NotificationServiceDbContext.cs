using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Persistence.Data
{
    public class NotificationServiceDbContext : DbContext
    {
        public NotificationServiceDbContext(DbContextOptions<NotificationServiceDbContext> options)
            : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notifications", "public");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(e => e.RequestId)
                    .IsRequired();

                entity.Property(e => e.RecipientId)
                    .IsRequired();

                entity.Property(e => e.Type)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasDefaultValue("Queued");

                entity.Property(e => e.Error)
                    .HasColumnType("text");

                entity.Property(e => e.SentAt)
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("now()");

                // Indexes
                entity.HasIndex(e => e.RecipientId)
                    .HasDatabaseName("idx_notifications_recipientid");

                entity.HasIndex(e => e.RequestId)
                    .HasDatabaseName("idx_notifications_requestid");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("idx_notifications_status");

                entity.HasIndex(e => e.Type)
                    .HasDatabaseName("idx_notifications_type");
            });
        }
    }
}

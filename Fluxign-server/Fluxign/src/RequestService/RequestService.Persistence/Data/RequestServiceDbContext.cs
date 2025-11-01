using Microsoft.EntityFrameworkCore;
using RequestService.Domain.Entities;
using RequestService.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RequestService.Persistence.Data
{
    public class RequestServiceDbContext : DbContext
    {
        public RequestServiceDbContext(DbContextOptions<RequestServiceDbContext> options) : base(options) { }

        public DbSet<SigningRequest> SigningRequests { get; set; }
        public DbSet<SigningRecipient> SigningRecipients { get; set; }
        public DbSet<SignaturePosition> SignaturePositions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<SigningRecipient>(entity =>
            {
                entity.Property(e => e.Metadata)
                      .HasColumnType("jsonb");

                entity.HasOne(e => e.Request)
                      .WithMany(x=>x.SigningRecipients)
                      .HasForeignKey(e => e.RequestId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.SigningToken)
                      .IsUnique();
            });

            modelBuilder.Entity<SignaturePosition>(entity =>
            {
                entity.HasIndex(e => new { e.RequestId, e.RecipientId })
                      .IsUnique()
                      .HasDatabaseName("idx_signaturepositions_request_recipient");

                entity.HasOne(e => e.Recipient)
                      .WithOne(r => r.SignaturePosition)
                      .HasForeignKey<SignaturePosition>(e => e.RecipientId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Request)
                      .WithMany(x => x.SignaturePositions)
                      .HasForeignKey(e => e.RequestId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}

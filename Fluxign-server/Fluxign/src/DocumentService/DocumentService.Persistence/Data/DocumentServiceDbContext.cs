using DocumentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Persistence.Data
{
    public class DocumentServiceDbContext: DbContext
    {
        public DocumentServiceDbContext(DbContextOptions<DocumentServiceDbContext> options) : base(options) { }

        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentVersion> DocumentVersions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("uuid-ossp");

            modelBuilder.Entity<DocumentVersion>(entity =>
            {
                entity.ToTable("Document Versions");
                entity.HasKey(u => u.Id);
                entity.HasOne(u => u.Documents)
                      .WithMany(r => r.DocumentVersions)
                      .HasForeignKey(u => u.Id)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}

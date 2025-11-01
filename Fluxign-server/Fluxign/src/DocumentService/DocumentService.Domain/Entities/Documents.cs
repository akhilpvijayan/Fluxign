using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Domain.Entities
{

    [Table("Documents", Schema = "public")]
    public class Document
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid RequestId { get; set; }

        public string OriginalFile { get; set; }

        public string? SignedFile { get; set; }

        public string OriginalFileName { get; set; }

        public long? OriginalFileSize { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }

        public ICollection<DocumentVersion> DocumentVersions { get; set; }
    }
}

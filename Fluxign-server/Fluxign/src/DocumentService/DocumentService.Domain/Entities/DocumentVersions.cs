using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Domain.Entities
{
    [Table("DocumentVersions", Schema = "public")]
    public class DocumentVersion
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid DocumentId { get; set; }

        public Document Documents { get; set; }

        [Required]
        public int VersionNumber { get; set; }

        [Required]
        public byte[] Document { get; set; }

        public DateTimeOffset? LastSigned { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}

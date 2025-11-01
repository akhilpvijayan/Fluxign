using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequestService.Domain.Enum;

namespace RequestService.Domain.Entities
{
    [Table("SigningRequests", Schema = "public")]
    public class SigningRequest
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid RequesterId { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string Status { get; set; }

        public bool IsSequentialSigning { get; set; } = false;

        [Required]
        [Range(0, int.MaxValue)]
        public int? TotalRecipients { get; set; }

        [Range(0, int.MaxValue)]
        public int CompletedSigners { get; set; } = 0;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? CompletedAt { get; set; }

        public bool IsActive { get; set; } = true;
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }

        public ICollection<SigningRecipient> SigningRecipients { get; set; }
        public ICollection<SignaturePosition> SignaturePositions { get; set; }
    }
}

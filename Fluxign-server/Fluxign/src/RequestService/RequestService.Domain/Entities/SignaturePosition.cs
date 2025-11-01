using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestService.Domain.Entities
{
    [Table("SignaturePositions", Schema = "public")]
    public class SignaturePosition
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid RequestId { get; set; }

        [ForeignKey(nameof(RequestId))]
        public SigningRequest Request { get; set; }

        [Required]
        public Guid RecipientId { get; set; }

        [Required]
        public decimal XPosition { get; set; }

        [Required]
        public decimal YPosition { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? CompletedAt { get; set; }

        public bool IsActive { get; set; } = true;
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }

        [ForeignKey(nameof(RecipientId))]
        public SigningRecipient Recipient { get; set; }
    }
}

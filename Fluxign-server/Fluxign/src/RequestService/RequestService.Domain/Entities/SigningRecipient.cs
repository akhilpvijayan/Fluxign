using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using RequestService.Domain.Enum;

namespace RequestService.Domain.Entities
{
    [Table("SigningRecipients", Schema = "public")]
    public class SigningRecipient
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid RequestId { get; set; }

        [ForeignKey(nameof(RequestId))]
        public SigningRequest Request { get; set; }

        [Required]
        public string RecipientName { get; set; }

        public string RecipientPhone { get; set; }

        [Required]
        public string RecipientEmail { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int SigningOrder { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string SigningToken { get; set; }

        public DateTimeOffset? LinkSentAt { get; set; }

        public DateTimeOffset? SignedAt { get; set; }

        public JsonObject? Metadata { get; set; } = null;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? CompletedAt { get; set; }

        public bool IsActive { get; set; } = true;
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }

        public SignaturePosition SignaturePosition { get; set; }
    }
}

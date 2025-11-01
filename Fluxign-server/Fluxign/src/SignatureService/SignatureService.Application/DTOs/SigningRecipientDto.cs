using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SignatureService.Application.DTOs
{
    public class SigningRecipientDto
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }
        public string RecipientName { get; set; }
        public string RecipientPhone { get; set; }
        public string RecipientEmail { get; set; }
        public int SigningOrder { get; set; }
        public string Status { get; set; }
        public string SigningToken { get; set; }
        public DateTimeOffset? LinkSentAt { get; set; }
        public DateTimeOffset? SignedAt { get; set; }
        public JsonObject? Metadata { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}

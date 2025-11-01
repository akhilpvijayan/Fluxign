using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestService.Application.DTOs
{
    public class DocumentDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid RequestId { get; set; }

        public string OriginalFile { get; set; }

        public string? SignedFile { get; set; }

        public string OriginalFileName { get; set; }

        public long? OriginalFileSize { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        public Guid? CreatedBy { get; set; }

        public Guid? UpdatedBy { get; set; }
    }
}

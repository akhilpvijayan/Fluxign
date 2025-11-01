using RequestService.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Application.ViewModels
{
    public class AddDocument
    {
        public Guid RequestId { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public string PdfBase64 { get; set; }
        public List<Signer> Signers { get; set; }
        public string Title { get; set; }
        public Guid UserId { get; set; }
        public int Status { get; set; }
        public string? Token { get; set; } = null;
    }

    public class Signer
    {
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Name { get; set; }
        public Position Position { get; set; }
    }

    public class Position
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int Page { get; set; }
    }
}

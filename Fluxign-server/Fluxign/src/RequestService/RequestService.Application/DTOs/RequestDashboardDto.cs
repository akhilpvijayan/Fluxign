using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestService.Application.DTOs
{
    public class RequestDashboardDto
    {
        public Guid RequestId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public List<RecipientDto> Recipients { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

    public class RecipientDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string AvatarUrl { get; set; }
        public bool Signed { get; set; }
        public bool Rejected { get; set; }
        public string Action { get; set; }
        public int Order { get; set; }
        public List<SignerPositionDto> Positions { get; set; }
    }

    public class SignerPositionDto
    {
        public int Page { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }

    public class UserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string AvatarImage { get; set; }
    }

}

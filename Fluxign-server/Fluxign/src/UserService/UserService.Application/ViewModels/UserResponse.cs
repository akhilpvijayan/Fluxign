using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.ViewModels
{
    public class UserResponse
    {
        public Guid Id { get; }
        public string EmiratesId { get; }
        public string Email { get; }
        public string? Phone { get; }
        public string? FirstName { get; }
        public string? LastName { get; }
        public string Role { get; }
        public bool IsActive { get; }
        public bool IsEmailVerified { get; }
        public DateTimeOffset CreatedAt { get; }

        public UserResponse(Guid id, string emiratesId, string email, string? phone, string? firstName, string? lastName,
            string role, bool isActive, bool isEmailVerified, DateTimeOffset createdAt)
        {
            Id = id;
            EmiratesId = emiratesId;
            Email = email;
            Phone = phone;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
            IsActive = isActive;
            IsEmailVerified = isEmailVerified;
            CreatedAt = createdAt;
        }
    }

}

using System;

namespace UserService.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EmiratesId { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Guid RoleId { get; set; }
        public UserRole Role { get; set; }

        public bool IsActive { get; set; } = true;
        public string Password { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? LastLogin { get; set; }
        public string? AvatarImage { get; set; } = null;
        public bool IsDelete { get; set; } = false;
        public bool IsEmailVerified { get; set; } = false;

        public ICollection<UserSession> Sessions { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; } = new();
        public ICollection<UserOtp> Otps { get; set; } = new List<UserOtp>();
        public ICollection<PasswordResetRequest> PasswordResetRequests { get; set; } = new List<PasswordResetRequest>();
    }

}

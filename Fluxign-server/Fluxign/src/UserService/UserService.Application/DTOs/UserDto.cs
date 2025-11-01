
using Microsoft.AspNetCore.Http;

public class UserDto
{
    public Guid? Id { get; set; }
    public string EmiratesId { get; set; }
    public string UserEmail { get; set; }
    public string UserPhone { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? AvatarImage { get; set; }
    public bool? IsEmailVerified { get; set; }
}

public class PasswordResetRequestModel
{
    public string Email { get; set; }
}

public class ResetPasswordModel
{
    public string Token { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}


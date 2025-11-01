public class Otp
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string OtpCode { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public string Purpose { get; set; }
}

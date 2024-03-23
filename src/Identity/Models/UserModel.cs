namespace Identity.Models;

public class UserModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    
    public string PasswordHash { get; set; } = default!;
    public string PasswordSalt { get; set; } = default!;

    
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }

    public string TFAAuthenticationKey { get; set; } = default!;
    public string TFAQrCode { get; set; } = default!;
    public string TFAManualKey { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public DateTime? DeletedDate { get; set; }
    public string? DeleteNote { get; set; }
}

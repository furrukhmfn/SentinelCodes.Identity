

namespace Identity.Models;

public class UserRoleClaimModel
{
    public int Id { get; set; }
    public Guid UserRoleId { get; set; }
    public string ClaimType { get; set; } = default!;
    public string ClaimValue { get; set; } = default!;
}

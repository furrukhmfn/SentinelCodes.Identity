
namespace Identity.Models;

internal class RoleClaimModel
{
    public int Id { get; set; }
    public Guid RoleId { get; set; }
    public string ClaimType { get; set; } = default!;
    public string ClaimValue { get; set;} = default!;
}

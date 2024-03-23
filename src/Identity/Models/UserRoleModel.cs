

namespace Identity.Models;

public class UserRoleModel
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}

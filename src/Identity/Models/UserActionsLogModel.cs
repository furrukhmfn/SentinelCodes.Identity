namespace Identity.Models;

public class UserActionsLogModel
{
    public int Id { get; set; }
    public int ActionId { get; set; }
    public Guid UserId { get; set; }
    public string Data { get; set; } = default!;
    public DateTime Date { get; set; }
}

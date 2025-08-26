namespace ToDo.Models;

public class Item
{
    public int Id { get; set; }
    public string Task { get; set; }
    public bool IsCompleted { get; set; } = false;
    public int UserId { get; set; }
    public User User { get; set; }
}

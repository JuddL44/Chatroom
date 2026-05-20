public class Conversation
{
    public Guid Id {get; set;}
    public Guid AdminId {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
}
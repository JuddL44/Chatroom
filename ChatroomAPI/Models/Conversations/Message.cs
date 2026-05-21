public class Message
{
    public Guid Id {get; set;}
    public Guid ConversationId {get; set;}
    public Guid SenderId {get; set;} 
    public string SenderName {get; set;} = null!;
    public string Content {get; set;} = null!;
    public bool Console {get; set;} = false;
    public DateTime SentAt {get; set;} = DateTime.UtcNow;
}
public class Conversation
{
    public Guid Id {get; set;}
    public Guid AdminId {get; set;}
    public string Color {get; set;} = "#776871";
    public string Icon {get; set;} = "📧";
    public string Name {get; set;} = "Conversation";
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
}
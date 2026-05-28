public class ConversationDTO
{
    public Guid Id {get; set;}
    public string LastMessage {get; set;} = null!;
    public string LastSender {get; set;} = null!;
    public string Icon {get; set;} = null!;
    public string Color {get; set;} = null!;
    public string Name {get; set;} = null!;
}
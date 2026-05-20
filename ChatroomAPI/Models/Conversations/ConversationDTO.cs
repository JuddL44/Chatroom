public class ConversationDTO
{
    public Guid Id {get; set;}
    public string LastMessage {get; set;} = null!;
    public string LastSender {get; set;} = null!;
}
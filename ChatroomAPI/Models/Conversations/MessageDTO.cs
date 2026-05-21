public class MessageDTO
{
    public string SenderName {get; set;} = null!; 
    public string Content {get; set;} = null!;
    public DateTime SentAt {get; set;} = DateTime.UtcNow;
    public Boolean Console {get; set;} = false;
}
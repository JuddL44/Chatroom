using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

public class GetConversationMessagesHandler : IRequestHandler<GetConversationMessagesQuery, List<MessageDTO>>
{
    private readonly AppDbContext _context;

    public GetConversationMessagesHandler(AppDbContext context)
    {
        _context = context;
    }

     public async Task<List<MessageDTO>> Handle(GetConversationMessagesQuery req, CancellationToken ct)
    {
        var messages = await _context.Messages.Where(m => m.ConversationId == req.ConvoId).Select(m => new MessageDTO {Content = m.Content, SenderName= m.SenderName, SentAt = m.SentAt, ConversationId = m.ConversationId}).ToListAsync();
        return messages;
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetUserConversationsHandler : IRequestHandler<GetUserConversationsQuery, List<ConversationDTO>>
{
    private readonly AppDbContext _context;

    public GetUserConversationsHandler(AppDbContext context)
    {
        _context = context;
    }

     public async Task<List<ConversationDTO>> Handle(GetUserConversationsQuery req, CancellationToken ct)
    {
            var conversations = await (
        from cp in _context.ConversationParticipants
        join c in _context.Conversations
            on cp.Conversationid equals c.Id
        where cp.UserId == req.UserId
        select new ConversationDTO
        {
            Id = c.Id,
            Color = c.Color,
            Icon = c.Icon,
            Name = c.Name
        }
    ).ToListAsync(ct);

        return conversations;
    }
}
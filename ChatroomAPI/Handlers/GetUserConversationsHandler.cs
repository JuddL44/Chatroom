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
        var conversations = await _context.ConversationParticipants
            .Where(x => x.UserId == req.UserId)
            .Select(x => new ConversationDTO
            {
                Id = x.Conversationid
            }).ToListAsync(ct);

        return conversations;
    }
}
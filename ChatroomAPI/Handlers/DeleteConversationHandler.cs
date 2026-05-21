using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class DeleteConversationHandler : IRequestHandler<DeleteConversationCommand>
{
    private readonly AppDbContext _context;
    private readonly IHubContext<ChatHub> _hub;

    public DeleteConversationHandler(AppDbContext context, IHubContext<ChatHub> hub)
    {
        _context = context;
        _hub = hub;
    }

    public async Task Handle(DeleteConversationCommand req, CancellationToken ct)
    {
        var convo = await _context.Conversations.FirstOrDefaultAsync(c => c.Id == req.ConversationId && c.AdminId == req.UserId, ct);
        if (convo == null) { throw new KeyNotFoundException("Conversation not found"); }
        var participants = await _context.ConversationParticipants.Where(p => p.Conversationid == req.ConversationId).ToListAsync(ct);
        _context.ConversationParticipants.RemoveRange(participants);
        _context.Conversations.Remove(convo);
        await _context.SaveChangesAsync(ct);
        await _hub.Clients.Group(convo.Id.ToString()).SendAsync("DeleteConversation", new
        {
        conversationId = convo.Id
        });
        return;
    }
}
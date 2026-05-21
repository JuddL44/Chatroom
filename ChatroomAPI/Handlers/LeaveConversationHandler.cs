using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class LeaveConversationHandler : IRequestHandler<LeaveConversationCommand>
{
    private readonly AppDbContext _context;

    public LeaveConversationHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(LeaveConversationCommand req, CancellationToken ct)
    {
        var participant = await _context.ConversationParticipants.SingleOrDefaultAsync(x => x.Conversationid == req.ConversationId && x.UserId == req.UserId, ct);
        if (participant == null) { return; }
        var conversation = await _context.Conversations.SingleOrDefaultAsync(x => x.Id == req.ConversationId, ct);
        if (conversation == null) { return; }

        if (conversation.AdminId == req.UserId) // We gotta make sure a new admin is assigned if it's the conversation admin thats leaving. :)
        {
            var newAdmin = await _context.ConversationParticipants.Where(x => x.Conversationid == req.ConversationId && x.UserId != req.UserId).FirstOrDefaultAsync(ct);
            if (newAdmin != null)
            {
                conversation.AdminId = newAdmin.UserId;
            }
            else
            {
                _context.Conversations.Remove(conversation);
            }
        }
        _context.ConversationParticipants.Remove(participant);
        await _context.SaveChangesAsync(ct);
    }
}
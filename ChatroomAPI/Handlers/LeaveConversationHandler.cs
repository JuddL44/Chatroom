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
        var participant = await _context.ConversationParticipants.FirstOrDefaultAsync(x => x.Conversationid == req.ConversationId && x.UserId == req.UserId, ct);
        if (participant == null) { return; }
        _context.ConversationParticipants.Remove(participant);
        await _context.SaveChangesAsync(ct);
    }
}
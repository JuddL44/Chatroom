using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class UpdateConversationHandler : IRequestHandler<UpdateConversationCommand>
{
    private readonly AppDbContext _context;
    public UpdateConversationHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateConversationCommand req, CancellationToken ct)
    {
        var convo = await _context.Conversations.FirstOrDefaultAsync(x => x.Id == req.ConversationId, ct);
        if (convo == null) { return; }
        convo.Color = req.Color;
        convo.Name = req.Name;
        convo.Icon = req.Icon;
        await _context.SaveChangesAsync(ct);
    }
}
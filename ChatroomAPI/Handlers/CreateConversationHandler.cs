using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class CreateConversationHandler : IRequestHandler<CreateConversationCommand, Guid>
{
    private readonly AppDbContext _context;
    private readonly IHubContext<ChatHub> _hub;
    public CreateConversationHandler(AppDbContext context, IHubContext<ChatHub> hub)
    {
        _context = context;
        _hub = hub;
    }

    public async Task<Guid> Handle(CreateConversationCommand req, CancellationToken ct)
    {
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            AdminId = req.CreatorId,
            Color = req.Color,
            Icon = req.Icon,
            Name = req.Name
        };

        _context.Conversations.Add(conversation);
        
        _context.ConversationParticipants.Add(new ConversationParticipant
        {
            Conversationid = conversation.Id,
            UserId = req.CreatorId
        });

        var addedParticipant = await _context.Users.FirstOrDefaultAsync(u => u.Username == req.TargetUsername);
        if (addedParticipant == null) { return Guid.Empty; }
        _context.ConversationParticipants.Add(new ConversationParticipant
        {
            Conversationid = conversation.Id,
            UserId = addedParticipant.Id
        });

        await _context.SaveChangesAsync(ct);
        await _hub.Clients
            .Users(req.CreatorId.ToString(), addedParticipant.Id.ToString())
    .SendAsync("ConversationUpdate", new
    {
        conversationId = conversation.Id,
        type = "created"
    });
        return conversation.Id;

    }
}
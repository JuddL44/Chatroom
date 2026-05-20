using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class CreateConversationHandler : IRequestHandler<CreateConversationCommand, Guid>
{
    private readonly AppDbContext _context;

    public CreateConversationHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateConversationCommand req, CancellationToken ct)
    {
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            AdminId = req.CreatorId
        };

        _context.Conversations.Add(conversation);
        
        _context.ConversationParticipants.Add(new ConversationParticipant
        {
            Conversationid = conversation.Id,
            UserId = req.CreatorId
        });

        _context.ConversationParticipants.Add(new ConversationParticipant
        {
            Conversationid = conversation.Id,
            UserId = req.TargetUserId
        });

        await _context.SaveChangesAsync(ct);
        return conversation.Id;

    }
}
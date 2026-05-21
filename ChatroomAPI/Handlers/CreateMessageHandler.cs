using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class CreateMessageHandler : IRequestHandler<CreateMessageCommand, Guid>
{
    private readonly AppDbContext _context;

    public CreateMessageHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateMessageCommand req, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == req.CreatorId, ct);
        if (user == null) { throw new Exception("User not found"); }
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = req.ConvoId,
            SenderId = req.CreatorId,
            SentAt = DateTime.UtcNow,
            Content = req.Message,
            SenderName = user.Username,
            Console = req.isConsole
        };
        _context.Messages.Add(message);
        await _context.SaveChangesAsync(ct);
        return message.Id;
    }
}
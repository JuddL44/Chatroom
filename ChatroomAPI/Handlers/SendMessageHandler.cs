using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

public class SendMessageHandler : IRequestHandler<SendMessageCommand>
{
    private readonly AppDbContext _context;
    private readonly IHubContext<ChatHub> _hub;
    public SendMessageHandler(AppDbContext context, IHubContext<ChatHub> hub)
    {
        _context = context;
        _hub = hub;
    }

    public async Task Handle(SendMessageCommand req, CancellationToken ct)
    {
        var isParticipant = await _context.ConversationParticipants.AnyAsync(x => x.Conversationid == req.ConversationId && x.UserId == req.SenderId, ct);
        if (!isParticipant) { throw new Exception("Not in the conversation"); }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = req.ConversationId,
            SenderId = req.SenderId,
            Content = req.Content,
            SentAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync(ct);

        await _hub.Clients.GroupExcept(req.ConversationId.ToString(), req.SenderId.ToString()).SendAsync("ReceiveMessage", new
        {
            conversationId = req.ConversationId,
            senderId = req.SenderId,
            content = req.Content
        }, ct);
    }



}
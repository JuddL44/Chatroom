using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMediator _mediator;
    private readonly AppDbContext _context;
    public ChatHub(IMediator mediator, AppDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }


    public async Task SendMessage(Guid conversationId, string message)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) { throw new Exception("Unauthorized"); }
        await _mediator.Send(new SendMessageCommand(Guid.Parse(userId), conversationId, message));
    }

    public async Task JoinConversation(Guid conversationId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) { throw new HubException("Unauthorized"); };

        var isMember = await _context.ConversationParticipants.AnyAsync(x => x.Conversationid == conversationId && x.UserId.ToString() == userId);
        if (!isMember) { throw new HubException("Not in conversation"); }

        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
    }
}
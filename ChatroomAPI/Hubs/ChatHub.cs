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

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"Connected: {Context.UserIdentifier}");
        var userId = Context.UserIdentifier;
        if (userId != null) { await Groups.AddToGroupAsync(Context.ConnectionId, userId); }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
            Console.WriteLine($"Disconnected WITH ERROR: {Context.UserIdentifier} | {exception.GetType().Name}: {exception.Message}");
        else
            Console.WriteLine($"Disconnected cleanly: {Context.UserIdentifier}");

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(Guid conversationId, string message)
    {
        try
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) throw new HubException("Unauthorized");

            await _mediator.Send(new CreateMessageCommand(Guid.Parse(userId), conversationId, message, false));
        }
        catch (HubException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SendMessage ERROR: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw new HubException("Failed to send message"); 
        }
    }

    public async Task JoinConversation(string conversationId)
    {
        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
            Console.WriteLine($"User {Context.UserIdentifier} joined group {conversationId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JoinConversation ERROR: {ex.GetType().Name}: {ex.Message}");
            throw new HubException("Failed to join conversation");
        }
    }
}
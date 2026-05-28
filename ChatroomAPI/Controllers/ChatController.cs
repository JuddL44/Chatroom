using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwt;
    private readonly IMediator _mediator;
    public ChatController(AppDbContext context, JwtService jwt, IMediator mediator)
    {
        _jwt = jwt;
        _context = context;
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await _mediator.Send(new GetUserConversationsQuery(Guid.Parse(userId!)));
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{convoId}/admin")]
    public async Task<IActionResult> GetConversationAdmin(Guid convoId)
    {
        var result = await _mediator.Send(new GetConversationAdminQuery(convoId));
        return Ok(result);
    }

    [Authorize]
    [HttpPost("conversation")]
    public async Task<IActionResult> CreateConversation(CreateConversationCommand command)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid userGuid = new Guid(userId!);
        var result = await _mediator.Send(command with { CreatorId = userGuid });
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{convoId}/leave")]
    public async Task<IActionResult> LeaveConversation(Guid convoId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid userGuid = new Guid(userId!);
        await _mediator.Send(new LeaveConversationCommand(convoId, userGuid));
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{convoId}/delete")]
    public async Task<IActionResult> DeleteConversation(Guid convoId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid userGuid = new Guid(userId!);
        await _mediator.Send(new DeleteConversationCommand(convoId, userGuid));
        return NoContent();
    }

    [Authorize]
    [HttpPost("{convoId}/message")]
    public async Task<IActionResult> CreateMessage(Guid convoId, [FromBody] CreateMessageRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid userGuid = new Guid(userId!);
        var result = await _mediator.Send(new CreateMessageCommand(userGuid, convoId, request.Message, false));
        return Ok(result);
    }

    [Authorize]
    [HttpPost("{convoId}/update")]
    public async Task<IActionResult> UpdateConversation(Guid convoId, [FromBody] UpdateConversationDto dto)
    {
         await _mediator.Send(new UpdateConversationCommand(
        convoId,
        dto.Color,
        dto.Icon,
        dto.Name));
        return Ok();
    }

    [Authorize]
    [HttpGet("{convoId}/messages")]
    public async Task<IActionResult> GetMessages(Guid convoId)
    {
        var result = await _mediator.Send(new GetConversationMessagesQuery(convoId));
        return Ok(result);
    }
    
}

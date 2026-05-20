using MediatR;

public record LeaveConversationCommand(Guid ConversationId, Guid UserId) : IRequest;
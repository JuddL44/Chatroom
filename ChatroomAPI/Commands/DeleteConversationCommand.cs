using MediatR;

public record DeleteConversationCommand(Guid ConversationId, Guid UserId) : IRequest;
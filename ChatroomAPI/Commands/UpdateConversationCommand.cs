using MediatR;

public record UpdateConversationCommand(Guid ConversationId, string Color, string Icon, string Name) : IRequest;
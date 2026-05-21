using MediatR;

public record CreateConversationCommand(Guid CreatorId, string TargetUsername) : IRequest<Guid>;
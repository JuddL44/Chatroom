using MediatR;

public record CreateConversationCommand(Guid CreatorId, Guid TargetUserId) : IRequest<Guid>;
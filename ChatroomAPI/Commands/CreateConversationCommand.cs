using MediatR;

public record CreateConversationCommand(Guid CreatorId, string TargetUsername, string Icon, string Color, string Name) : IRequest<Guid>;
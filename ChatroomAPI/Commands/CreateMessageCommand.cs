using MediatR;

public record CreateMessageCommand(Guid CreatorId, Guid ConvoId, string Message, bool isConsole) : IRequest<Guid>;
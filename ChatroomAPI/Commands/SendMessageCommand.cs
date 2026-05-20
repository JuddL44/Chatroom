using MediatR;
public record SendMessageCommand(Guid SenderId, Guid ConversationId, string Content): IRequest;
using MediatR;
public record GetConversationMessagesQuery(Guid ConvoId) : IRequest<List<MessageDTO>>;
using MediatR;

public record GetUserConversationsQuery(Guid UserId) : IRequest<List<ConversationDTO>>;
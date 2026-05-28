using MediatR;
public record GetConversationAdminQuery(Guid ConvoId) : IRequest<string>;
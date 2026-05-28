using MediatR;

public record GetUsernameQuery(Guid UserId) : IRequest<string>;
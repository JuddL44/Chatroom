using MediatR;
public record RegisterUserCommand(string Username, string Password) : IRequest<Guid>;
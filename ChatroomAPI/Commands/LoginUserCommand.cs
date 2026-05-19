using MediatR;
public record LoginUserCommand(string Username, string Password) : IRequest<string>;
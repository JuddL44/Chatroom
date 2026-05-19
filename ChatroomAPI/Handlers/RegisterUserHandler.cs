using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly AppDbContext _context;
    public RegisterUserHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RegisterUserCommand req, CancellationToken ct)
    {
        var exists = await _context.Users.AnyAsync(x => x.Username == req.Username);
        if (exists) { throw new Exception("Username already exists"); }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = req.Username,
            PasswordHash = PasswordHasher.Hash(req.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);
        return user.Id;
        }
}
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, string>
{
    private readonly AppDbContext _context;
        private readonly JwtService _jwt;
    public LoginUserHandler(AppDbContext context, JwtService jwt)
    {
        _context = context;
        _jwt = jwt;
    }

    public async Task<string> Handle(LoginUserCommand req, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == req.Username);
        if (user == null) { throw new Exception("Invalid credentials"); }

        var valid = PasswordHasher.Verify(req.Password, user.PasswordHash);

        if (!valid) { throw new Exception("Incorrect password"); }

        return _jwt.GenerateToken(user);
        }
}
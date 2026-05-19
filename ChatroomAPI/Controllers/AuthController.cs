using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwt;
    public AuthController(AppDbContext context, JwtService jwt)
    {
        _jwt = jwt;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        var exists = await _context.Users.AnyAsync(x => x.Username == req.Username);
        if (exists) { return BadRequest("Username already exists"); }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = req.Username,
            PasswordHash = PasswordHasher.Hash(req.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User created "});
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == req.Username);
        if (user == null) { return NotFound("Invalid credentials"); }

        var valid = PasswordHasher.Verify(req.Password, user.PasswordHash);

        if (!valid) { return Unauthorized("Incorrect password"); }

        var token = _jwt.GenerateToken(user);

        return Ok(new { token } );
    }


    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            username = User.Identity?.Name
        });
    }
}
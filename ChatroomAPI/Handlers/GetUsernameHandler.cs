using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetUsernameHandler : IRequestHandler<GetUsernameQuery, string>
{
    private readonly AppDbContext _context;

    public GetUsernameHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(GetUsernameQuery req, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(w => w.Id == req.UserId);
        string username = "";
        if (user != null)
        {
        username = user.Username;
        }
        return username;
    }
     
}
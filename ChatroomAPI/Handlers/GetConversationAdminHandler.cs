using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

public class GetConversationAdminHandler : IRequestHandler<GetConversationAdminQuery, string>
{
    private readonly AppDbContext _context;

    public GetConversationAdminHandler(AppDbContext context)
    {
        _context = context;
    }

     public async Task<string> Handle(GetConversationAdminQuery req, CancellationToken ct)
    {
        var convo = await _context.Conversations.FirstOrDefaultAsync(x => x.Id == req.ConvoId, ct);
        if (convo == null) {return "";}
        var admin = await _context.Users.FirstOrDefaultAsync(u => u.Id == convo.AdminId, ct);
        if (admin == null || admin.Username == null) {return "";}
        return admin.Username;
    }
}
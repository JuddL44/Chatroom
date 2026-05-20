using Microsoft.EntityFrameworkCore;


public class AppDbContext : DbContext
{
    public DbSet<User> Users {get; set;}
    public DbSet<Conversation> Conversations {get; set;}
    public DbSet<ConversationParticipant> ConversationParticipants {get; set;}
    public DbSet<Message> Messages {get; set;}
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       modelBuilder.Entity<ConversationParticipant>().HasIndex(x => x.UserId);
       modelBuilder.Entity<ConversationParticipant>().HasIndex(x => x.Conversationid); 
       modelBuilder.Entity<Message>().HasIndex(x => x.ConversationId);
    }
}
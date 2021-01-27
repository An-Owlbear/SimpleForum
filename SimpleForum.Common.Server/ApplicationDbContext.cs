using Microsoft.EntityFrameworkCore;
using SimpleForum.Models;

namespace SimpleForum.Common.Server
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserComment> UserComments { get; set; }
        public DbSet<AuthToken> AuthTokens { get; set; }
        public DbSet<EmailCode> EmailCodes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<OutgoingServerToken> OutgoingServerTokens { get; set; }
        public DbSet<IncomingServerToken> IncomingServerTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailCode>().HasKey(x => x.Code);
            
            modelBuilder.Entity<Thread>().HasIndex(x => x.DatePosted);
            modelBuilder.Entity<Comment>().HasIndex(x => x.DatePosted);
            modelBuilder.Entity<UserComment>().HasIndex(x => x.DatePosted);
        }
    }
}
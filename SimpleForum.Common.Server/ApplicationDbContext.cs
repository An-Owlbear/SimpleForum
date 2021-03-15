using Microsoft.EntityFrameworkCore;
using SimpleForum.Models;

namespace SimpleForum.Common.Server
{
    // Represents the database
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserComment> UserComments { get; set; }
        public DbSet<RemoteAuthToken> RemoteAuthTokens { get; set; }
        public DbSet<EmailCode> EmailCodes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<OutgoingServerToken> OutgoingServerTokens { get; set; }
        public DbSet<IncomingServerToken> IncomingServerTokens { get; set; }
        public DbSet<TempApiToken> TempApiTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmailCode>().HasKey(x => x.Code);
            
            modelBuilder.Entity<Thread>().HasIndex(x => x.DatePosted);
            modelBuilder.Entity<Comment>().HasIndex(x => x.DatePosted);
            modelBuilder.Entity<UserComment>().HasIndex(x => x.DatePosted);

            modelBuilder.Entity<User>()
                .HasOne(x => x.Server)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.ServerID)
                .IsRequired(false);
        }
    }
}
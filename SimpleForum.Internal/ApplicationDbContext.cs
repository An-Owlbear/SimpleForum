using Microsoft.EntityFrameworkCore;
using SimpleForum.Models;

namespace SimpleForum.Internal
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
        
        public DbSet<User> Users { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<UserComment> UserComments { get; set; }
    }
}
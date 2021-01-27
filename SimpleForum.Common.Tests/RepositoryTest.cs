using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SimpleForum.Common.Server;
using SimpleForum.Models;
using Xunit;

namespace SimpleForum.Common.Tests
{
    public class RepositoryTest : IDisposable
    {
        private readonly SimpleForumRepository repository;
        private readonly DbConnection _connection;

        public RepositoryTest()
        {
            // Creates ApplicationDbContext from in memory database
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(CreateInMemory()).Options;

            _connection = RelationalOptionsExtension.Extract(options).Connection;
            ApplicationDbContext context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            
            // Creates repository and adds data
            repository = new SimpleForumRepository(context, null, null, null);
            repository.AddUserAsync(new User() {Username = "user1", Password = "userpass", Email = "asp@asp.net"}).Wait();
            repository.SaveChangesAsync().Wait();
            repository.AddThreadAsync(new Thread() {Title = "first thread", Content = "Thread content", UserID = 1}).Wait();
            repository.SaveChangesAsync().Wait();
        }

        // Creates an in memory sqlite database
        public static DbConnection CreateInMemory()
        {
            SqliteConnection connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        }
        
        [Fact]
        public async Task TestGetUser()
        {
            User user = await repository.GetUserAsync(1);
            Assert.Equal("user1", user.Username);
            Assert.Equal("asp@asp.net", user.Email);
        }

        public void Dispose() => _connection.Close();
    }
}
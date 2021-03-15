using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace SimpleForum.Common.Server
{
    // Uses to connect to the server when using dotnet ef
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            string connectionString = Environment.GetEnvironmentVariable("DbConnectionString");
            
            if (connectionString == null) throw new NullReferenceException();
            
            builder.UseMySql(connectionString, x => x.ServerVersion(new Version(10, 4, 12), ServerType.MariaDb));
            builder.UseLazyLoadingProxies();
            return new ApplicationDbContext(builder.Options);
        }
    }
}
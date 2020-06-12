using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace SimpleForum.Internal
{
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(@Directory.GetCurrentDirectory() + "/../SimpleForum.Web/appsettings.json").Build();
            
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            string connectionString = configurationRoot.GetConnectionString("DatabaseConnection");
            builder.UseMySql(connectionString, x => x.ServerVersion(new Version(10, 4, 12), ServerType.MariaDb));
            builder.UseLazyLoadingProxies();
            return new ApplicationDbContext(builder.Options);
        }
    }
}
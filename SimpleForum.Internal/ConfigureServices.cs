using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

namespace SimpleForum.Internal
{
    public static class ConfigureServices
    {
        // Adds services used by all SimpleForum applications
        public static IServiceCollection AddSimpleForum(this IServiceCollection services, IConfiguration configuration)
        {
            // Sets environment variables
            string dbConnectionString = Environment.GetEnvironmentVariable("DbConnectionString");
            string[] mailConnectionStrings = Environment.GetEnvironmentVariable("MailConnectionString")?.Split(";");
            if (dbConnectionString == null || mailConnectionStrings == null) throw new NullReferenceException();
            
            // Adds configuration, database, and email
            services.Configure<SimpleForumConfig>(configuration);
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(dbConnectionString).UseLazyLoadingProxies());
            services.AddScoped<SimpleForumRepository>();
            services.AddMailKit(options => options.UseMailKit(new MailKitOptions()
            {
                Server = mailConnectionStrings[0].Trim(),
                Port = int.Parse(mailConnectionStrings[1].Trim()),
                SenderName = mailConnectionStrings[2].Trim(),
                SenderEmail = mailConnectionStrings[3].Trim(),
                Account = mailConnectionStrings[4].Trim(),
                Password = mailConnectionStrings[5].Trim(),
                Security = true
            }));
            
            // Configures headers for use with reverse proxy
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            return services;
        }
    }
}
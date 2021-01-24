using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using SimpleForum.API.Models.Responses;
using SimpleForum.Internal;

namespace SimpleForum.CrossConnection
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string dbConnectionString = Environment.GetEnvironmentVariable("DbConnectionString");
            string[] mailConnectionStrings = Environment.GetEnvironmentVariable("MailConnectionString")?.Split(";");
            if (dbConnectionString == null || mailConnectionStrings == null) throw new NullReferenceException();
            
            services.Configure<SimpleForumConfig>(Configuration.GetSection("SimpleForumConfig"));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(dbConnectionString).UseLazyLoadingProxies());
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

            services.AddScoped<SimpleForumRepository>();
            services.AddScoped<AuthenticationHandler>();

            
            services.AddAuthentication(AuthenticationOptions.Scheme)
                .AddScheme<AuthenticationOptions, AuthenticationHandler>(AuthenticationOptions.Scheme, null);

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddHttpContextAccessor();
            
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    Error error = new Error()
                    {
                        Message = context.ModelState.First().Value.Errors.First().ErrorMessage,
                        Type = 400
                    };
                    return new ObjectResult(error) { StatusCode = error.Type };
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using SimpleForum.Internal;
using SimpleForum.Web.Policies;

namespace SimpleForum.Web
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
            string[] mailConnectionStrings = Environment.GetEnvironmentVariable("MailConnectionString").Split(";");
            if (dbConnectionString == null) throw new NullReferenceException();

            services.Configure<SimpleForumConfig>(Configuration.GetSection("SimpleForumConfig"));
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = new PathString("/Login");
                    options.AccessDeniedPath = new PathString("/Home/Forbidden");
                });

            services.AddHttpContextAccessor();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ThreadOwnerOrAdmin", policy =>
                {
                    policy.Requirements.Add(new ThreadOwnerOrAdminRequirement());
                });
                options.AddPolicy("ThreadOwner", policy =>
                {
                    policy.Requirements.Add(new ThreadOwnerRequirement());
                });
                options.AddPolicy("ThreadReply", policy =>
                {
                    policy.Requirements.Add(new ThreadReplyRequirement());
                });
                options.AddPolicy("UserOwner", policy =>
                {
                    policy.Requirements.Add(new UserOwnerRequirement());
                });
                options.AddPolicy("UserOwnerOrAdmin", policy =>
                {
                    policy.Requirements.Add(new UserOwnerOrAdminRequirement());
                });
                options.AddPolicy("UserPageReply", policy =>
                {
                    policy.Requirements.Add(new UserPageReplyRequirement());
                });
                options.AddPolicy("CommentOwner", policy =>
                {
                    policy.Requirements.Add(new CommentOwnerRequirement());
                });
            });
            services.AddScoped<IAuthorizationHandler, ThreadOwnerOrAdminHandler>();
            services.AddScoped<IAuthorizationHandler, ThreadOwnerHandler>();
            services.AddScoped<IAuthorizationHandler, ThreadReplyHandler>();
            services.AddScoped<IAuthorizationHandler, RolesAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, UserOwnerHandler>();
            services.AddScoped<IAuthorizationHandler, UserOwnerOrAdminHandler>();
            services.AddScoped<IAuthorizationHandler, UserPageReplyHandler>();
            services.AddScoped<IAuthorizationHandler, CommentOwnerHandler>();

            services.AddScoped<VerifiedEmail>();
            services.AddScoped<CheckPassword>();

            services.AddControllersWithViews();
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Home/StatusError", "?code={0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRevokeBannedUsers();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
            });
        }
    }
}
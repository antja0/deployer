using System;
using System.Linq;
using Antja.Authentication.HMAC;
using AspNetCoreRateLimit;
using Deployer.Data;
using Deployer.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Deployer.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMemoryCache();

            // Rate limiting just in case - if it is not done at ingress level.
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimit"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.AddHttpContextAccessor();

            services.ConfigureDictionary<HMACSignatureOptions>(Configuration.GetSection("AuthOptions"));

            services.AddAuthentication(o => { o.DefaultScheme = "Nodes"; })
                .AddScheme<HMACSignatureHandler>("Nodes")
                .AddScheme<HMACSignatureHandler>("Webhook");

            services.AddDbContext<DeployerContext>(Configuration.GetConnectionString("Deployer"));

            services.Configure<DeployerOptions>(Configuration.GetSection("Deployer"));
            services.AddHttpClient();
            services.AddSingleton<IDeployerService, DeployerService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIpRateLimiting();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            UpdateDatabase(app);
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<DeployerContext>();
            context.Database.EnsureCreated();
            context.Database.Migrate();

            if (!context.Events.Any())
            {
                // Add default events:
                context.Events.Add(new Event { EventId = "push", Id = Guid.NewGuid().ToString(), ListNewVersions = false });
                context.Events.Add(new Event { EventId = "pull-request", Id = Guid.NewGuid().ToString(), ListNewVersions = false });
                context.Events.Add(new Event { EventId = "release", Id = Guid.NewGuid().ToString(), ListNewVersions = true });

                context.SaveChanges();
            }
        }
    }
}

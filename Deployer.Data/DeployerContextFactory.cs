using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Deployer.Data
{
    // ReSharper disable once UnusedMember.Global
    public class DeployerContextFactory : IDesignTimeDbContextFactory<DeployerContext>
    {
        DeployerContext IDesignTimeDbContextFactory<DeployerContext>.CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.Development.json", true)
                .Build();

            var dbContextBuilder = new DbContextOptionsBuilder<DeployerContext>();

            var connectionString = configuration.GetConnectionString("Deployer");

            // ReSharper disable once CommentTypo
            // TODO: Add support for postgre etc. if needed.
            dbContextBuilder.UseSqlServer(connectionString, o => o.MigrationsAssembly("Deployer.Data"));

            return new DeployerContext(dbContextBuilder.Options);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Deployer.Data
{
    public static class Extensions
    {
        public static IServiceCollection AddDbContext<TDbContext>(this IServiceCollection services, string connectionString)
            where TDbContext : DbContext
        {
            // ReSharper disable once CommentTypo
            // TODO: Add support for postgre etc. if needed.
            return services.AddDbContext<TDbContext>(options => options.UseSqlServer(connectionString, x => x.MigrationsAssembly("Deployer.Data")));
        }
    }
}

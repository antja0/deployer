using Deployer.Api.Nodes;
using Microsoft.EntityFrameworkCore;

namespace Deployer.Api
{
    public class DeployerContext : DbContext
    {
        public DbSet<Node> Nodes { get; set; }

        public DeployerContext(DbContextOptions<DeployerContext> options) : base(options)
        {
        }
    }
}

using Deployer.Models;
using Microsoft.EntityFrameworkCore;

namespace Deployer.Api.Data
{
    public class DeployerContext : DbContext
    {
        public DbSet<Node> Nodes { get; set; }

        public DeployerContext(DbContextOptions<DeployerContext> options) : base(options)
        {
        }
    }
}

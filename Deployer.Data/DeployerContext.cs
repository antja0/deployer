using Deployer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Deployer.Data
{
    public class DeployerContext : DbContext
    {
        public DbSet<Node> Nodes { get; set; }

        public DeployerContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Node>().HasQueryFilter(i => i.Registered && !i.Deleted);
        }
    }
}

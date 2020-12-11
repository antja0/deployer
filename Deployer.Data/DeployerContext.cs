using Deployer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Deployer.Data
{
    public class DeployerContext : DbContext
    {
        public DbSet<Node> Nodes { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Version> ApplicationVersions { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<DeploymentGroup> DeploymentGroups { get; set; }
        public DbSet<DeploymentRule> DeploymentRules { get; set; }

        public DeployerContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Node>().HasQueryFilter(i => i.Registered && !i.Deleted);

            builder.Entity<Application>().HasQueryFilter(i => !i.Deleted);

            builder.Entity<Version>().HasIndex(i => new {i.Version, i.ApplicationId}).IsUnique();

            builder.Entity<DeploymentRule>().HasKey(i => new {i.Type, i.DeploymentGroupId, i.ApplicationId});
        }
    }
}

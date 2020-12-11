using Deployer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Deployer.Data
{
    public class DeployerContext : DbContext
    {
        public DbSet<Node> Nodes { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Version> Versions { get; set; }
        public DbSet<Event> Events { get; set; }
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

            builder.Entity<Version>()
                .HasOne(i => i.Application)
                .WithMany(i => i.Versions)
                .HasForeignKey(i => i.ApplicationId);
            builder.Entity<Version>()
                .HasOne(i => i.Event)
                .WithMany(i => i.Versions)
                .HasForeignKey(i => i.EventId);
            builder.Entity<Version>().HasIndex(i => new {i.Name, i.ApplicationId, i.EventId}).IsUnique();

            builder.Entity<Event>().HasIndex(i => i.EventId).IsUnique();

            builder.Entity<DeploymentRule>()
                .HasOne(i => i.Event)
                .WithMany(i => i.DeploymentRules)
                .HasForeignKey(i => i.EventId);
            builder.Entity<DeploymentRule>()
                .HasOne(i => i.DeploymentGroup)
                .WithMany(i => i.DeploymentRules)
                .HasForeignKey(i => i.DeploymentGroupId);
            builder.Entity<DeploymentRule>()
                .HasOne(i => i.Application)
                .WithMany(i => i.DeploymentRules)
                .HasForeignKey(i => i.ApplicationId);
            builder.Entity<DeploymentRule>().HasKey(i => new {i.EventId, i.DeploymentGroupId, i.ApplicationId});
        }
    }
}

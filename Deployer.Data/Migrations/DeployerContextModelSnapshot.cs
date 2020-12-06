﻿// <auto-generated />
using Deployer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Deployer.Data.Migrations
{
    [DbContext(typeof(DeployerContext))]
    partial class DeployerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Deployer.Data.Models.Node", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(36)
                        .HasColumnType("CHAR(36)");

                    b.Property<string>("ApiEndpoint")
                        .HasMaxLength(256)
                        .HasColumnType("NVARCHAR(256)");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<string>("Description")
                        .HasMaxLength(1024)
                        .HasColumnType("NVARCHAR(1024)");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("NVARCHAR(128)");

                    b.Property<bool>("Registered")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Nodes");
                });
#pragma warning restore 612, 618
        }
    }
}

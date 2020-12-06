using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deployer.Data.Models
{
    public class Application
    {
        [Column(TypeName = "CHAR")]
        [StringLength(36)]
        public string Id { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(128)]
        public string Name { get; set; }

        /// <summary>
        /// Deployed application version endpoint.
        /// eg. https://localhost:1337/version.txt
        /// </summary>
        [Column(TypeName = "NVARCHAR")]
        [StringLength(256)]
        public string VersionEndpoint { get; set; }

        /// <summary>
        /// eg. github repository clone url.
        /// </summary>
        [Column(TypeName = "NVARCHAR")]
        [StringLength(256)]
        public string RepositoryUrl { get; set; }

        /// <summary>
        /// Path from repository root to changelog. Typically just CHANGELOG.md.
        /// </summary>
        [Column(TypeName = "NVARCHAR")]
        [StringLength(256)]
        public string ChangelogPath { get; set; }

        /// <summary>
        /// Path from repository root to script that will run pre-deploy.
        /// </summary>
        [Column(TypeName = "NVARCHAR")]
        [StringLength(128)]
        public string ScriptPath { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(1024)]
        public string Description { get; set; }

        public bool Registered { get; set; }

        public bool Deleted { get; set; }

        public List<ApplicationVersion> Versions { get; set; }
        public List<Node> Nodes { get; set; }
    }
}

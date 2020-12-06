using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deployer.Data.Models
{
    public class DeploymentGroup
    {
        [Column(TypeName = "CHAR")]
        [StringLength(36)]
        public string Id { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(512)]
        public string Name { get; set; }

        public List<Node> Nodes { get; set; }

        public List<Project> Projects { get; set; }

        /// <summary>
        /// Whether to add new nodes and projects to this list automatically when they are added.
        /// </summary>
        public bool UpdateAutomatically { get; set; }
    }
}

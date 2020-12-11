using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deployer.Data.Models
{
    public class Project
    {
        [Column(TypeName = "CHAR")]
        [StringLength(36)]
        public string Id { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(512)]
        public string Name { get; set; }

        public Node Node { get; set; }
        public List<Version> Versions { get; set; }
        public List<DeploymentGroup> DeploymentGroups { get; set; }
    }
}

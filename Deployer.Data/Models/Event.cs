using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deployer.Data.Models
{
    public class Event
    {
        [Column(TypeName = "CHAR")]
        [StringLength(36)]
        public string Id { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(64)]
        public string EventId { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(1024)]
        public string Description { get; set; }

        public bool ListNewVersions { get; set; }

        public List<Version> Versions { get; set; }

        public List<DeploymentRule> DeploymentRules { get; set; }
    }
}

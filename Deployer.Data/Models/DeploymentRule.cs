using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deployer.Data.Models
{
    public class DeploymentRule
    {
        [Column(TypeName = "CHAR")]
        [StringLength(36)]
        public string EventId { get; set; }

        [Column(TypeName = "CHAR")]
        [StringLength(36)]
        public string DeploymentGroupId { get; set; }

        [Column(TypeName = "CHAR")]
        [StringLength(36)]
        public string ApplicationId { get; set; }

        public Event Event { get; set; }

        public DeploymentGroup DeploymentGroup { get; set; }

        public Application Application { get; set; }

        public bool DeployAutomatically { get; set; }
    }
}

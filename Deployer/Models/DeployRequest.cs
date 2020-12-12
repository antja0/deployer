using System.Collections.Generic;

namespace Deployer.Models
{
    /// <summary>
    /// Sent to node to trigger deployment sequence.
    /// </summary>
    public class DeployRequest
    {
        public string VersionGuid { get; set; }
        public List<string> Projects { get; set; }
    }
}

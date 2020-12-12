using System.Threading.Tasks;
using Deployer.Data.Models;

namespace Deployer
{
    public interface IDeployerService
    {
        Task DeployAsync(Version version, DeploymentGroup deploymentGroup);
        Task<Version> BuildNewVersionAsync(Application application);
    }
}

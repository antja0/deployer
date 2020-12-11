using Deployer.Data.Models;

namespace Deployer
{
    public interface IDeployerService
    {
        Version BuildNewVersion(Application application);
    }
}

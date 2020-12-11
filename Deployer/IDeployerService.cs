using System.Threading.Tasks;
using Deployer.Data.Models;

namespace Deployer
{
    public interface IDeployerService
    {
        Task<Version> BuildNewVersionAsync(Application application);
    }
}

namespace Deployer.Data
{
    public enum DeploymentType
    {
        Push = 1,
        PullRequest = 2,
        Release = 3,
        Undefined = 404,
    }
}

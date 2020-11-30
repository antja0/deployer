namespace Deployer.Api.Nodes
{
    public class Node
    {
        public string Name { get; set; }
        public string ApiEndpoint { get; set; }
        public string Description { get; set; }
        public bool Registered { get; set; }
    }
}

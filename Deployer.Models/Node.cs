namespace Deployer.Models
{
    public class Node
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ApiEndpoint { get; set; }
        public string Description { get; set; }
        public bool Registered { get; set; }
    }
}

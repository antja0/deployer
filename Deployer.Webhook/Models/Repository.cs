using System.Text.Json.Serialization;

namespace Deployer.Webhook.Models
{
    public class Repository
    {
        public string Name { get; set; }

        /// <remarks>
        /// eg. "organization/repository"
        /// </remarks>
        [JsonPropertyName("full_name")]
        public string FullName { get; set; }
    }
}

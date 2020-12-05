using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deployer.Data.Models
{
    public class Node
    {
        [Column(TypeName = "CHAR")]
        [StringLength(36)]
        public string Id { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(128)]
        public string Name { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(256)]
        public string ApiEndpoint { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(1024)]
        public string Description { get; set; }


        public bool Registered { get; set; }
    }
}

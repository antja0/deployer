using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deployer.Data.Models
{
    public class Version
    {
        [Column(TypeName = "CHAR")]
        [StringLength(36)]
        public string Id { get; set; }

        /// <summary>
        /// eg. "v1.0.0"
        /// </summary>
        [Column(TypeName = "CHAR")]
        [StringLength(16)]
        public string Name { get; set; }

        public DateTime? Date { get; set; }

        public bool UnListed { get; set; }

        [Column(TypeName = "CHAR")]
        [StringLength(36)]
        public string EventId { get; set; }

        [Column(TypeName = "CHAR")]
        [StringLength(36)]
        public string ApplicationId { get; set; }

        public Event Event { get; set; }
        public Application Application { get; set; }
        public List<Project> Projects { get; set; }
    }
}

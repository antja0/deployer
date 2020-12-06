using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Deployer.Data.Models
{
    public class ApplicationVersion
    {
        [Column(TypeName = "CHAR")]
        [StringLength(16)]
        public string Version { get; set; }

        [Column(TypeName = "CHAR")]
        [StringLength(36)]
        public string ApplicationId { get; set; }
        
        public DateTime? Date { get; set; }

        public bool UnListed { get; set; }

        public Application Application { get; set; }
        public List<Project> Projects { get; set; }
    }
}

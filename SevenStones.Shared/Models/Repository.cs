using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenStones.Models
{
    [Index("Guid", IsUnique = true)]
    public class Repository
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SourceSystemId { get; set; }
        [MaxLength(1024)]
        public string Url { get; set; }
        [MaxLength(1024)]
        public string RemoteUrl { get; set; }
        [MaxLength(1024)]
        public string Name { get; set; }
        [MaxLength(1024)]
        public string Guid { get; set; }

        public virtual SourceSystem SourceSystem { get; set; }  
        public virtual List<RepositoryBranch> RepositoryBranches { get; set; }
    }

    public class RepositoryBranch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int RepositoryId { get; set; }
        [MaxLength(1024)]
        public string Name { get; set; }
        [MaxLength(1024)]
        public string LastCommit { get; set; }
        public DateTime LastCommitDateUtc { get; set; }

        public virtual Repository Repository { get; set; }
        public virtual List<Fact> Facts { get; set; }   
    }
}

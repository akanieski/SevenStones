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
    public enum FactType : int
    {
        Unknown                 = 0000,
        SecretsViolation        = 1000,
        DotnetTargetFramework   = 2000,
        DotnetOutputType        = 2001,
        BigFile                 = 3000,
        BlacklistedPath         = 3001
    }

    public class Fact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int RepositoryBranchId { get; set; }
        

        public FactType FactType { get; set; }
        [MaxLength(1024)]
        public string Path { get; set; } = "";
        [MaxLength(1024)]
        public string Value { get; set; } = "";
        [MaxLength(1024)]
        public string Location { get; set; } = "";
        [MaxLength(1024)]
        [AllowNull]
        public string ActionLink { get; set; }
        public virtual RepositoryBranch RepositoryBranch { get; set; }

    }
}

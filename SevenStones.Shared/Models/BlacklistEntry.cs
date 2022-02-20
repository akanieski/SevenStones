using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenStones.Models
{
    public class BlacklistEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(1024)]
        public string Name { get; set; }

        [Required]
        [MaxLength(4096)]
        public string Pattern { get; set; }

        [Required]
        public bool IsExclusion { get; set; } = false;
    }
}

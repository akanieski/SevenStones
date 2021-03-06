using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SevenStones.Models
{
    public class SourceSystem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(1024)]
        public string Guid { get; set; }
        [MaxLength(1024)]
        public string Name { get; set; }
        [MaxLength(1024)]
        public string AccessToken { get; set; }
    }
}

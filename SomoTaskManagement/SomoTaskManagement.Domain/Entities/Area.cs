using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Entities
{
    [Table("Area")]
    public class Area : BaseEntity
    {
        public Area()
        {
            Zones = new HashSet<Zone>();
        }
        public string Code { set; get; }
        
        [Required(ErrorMessage = "Area is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "FarmArea must be greater than 0.")]
        public double FArea { get; set; }

        public int FarmId { get; set; }

        [JsonIgnore]
        public virtual Farm? Farm { set; get; }
        [JsonIgnore]
        public virtual ICollection<Zone> Zones { set; get; }
    }
}

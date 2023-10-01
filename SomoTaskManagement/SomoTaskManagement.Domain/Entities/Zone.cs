using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Entities
{
    [Table("Zone")]

    public class Zone : BaseEntity
    {
        public Zone()
        {
            Fields = new HashSet<Field>();
        }

        [Required(ErrorMessage = "Area is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "FarmArea must be greater than 0.")]
        public double FarmArea { get; set; }

        public int ZoneTypeId { get; set; }

        public int AreaId { get; set; }


        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Area? Area { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ZoneType? ZoneType { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<Field> Fields { get; set; }
    }
}

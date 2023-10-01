using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Entities
{
    [Table("ZoneType")]

    public class ZoneType:BaseEntity
    {
        public ZoneType()
        {
            Zones = new HashSet<Zone>();
        }
        [JsonIgnore]
        public virtual ICollection<Zone> Zones { set; get; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Entities
{
    [Table("Other")]

    public class Other:BaseEntity
    {
        public Other()
        {
            Tasks = new HashSet<FarmTask>();
        }
        [JsonIgnore]
        public virtual ICollection<FarmTask> Tasks { get; set; }    
    }
}

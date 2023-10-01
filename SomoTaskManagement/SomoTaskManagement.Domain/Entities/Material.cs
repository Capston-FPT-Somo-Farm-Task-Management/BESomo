using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Entities
{
    [Table("Material")]
    public class Material:BaseEntity
    {
        public Material()
        {
            MaterialTasks = new HashSet<Material_Task>();
        }
        [JsonIgnore]
        public virtual ICollection<Material_Task> MaterialTasks { get; set; }
    }
}


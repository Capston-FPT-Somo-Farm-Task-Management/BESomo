using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Entities
{
    [Table("Role")]

    public class Role:BaseEntity
    {
        public Role()
        {
            Members = new HashSet<Member>();
        }
        [JsonIgnore]
        public virtual ICollection<Member> Members { get; set; }    
    }
}

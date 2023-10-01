using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Entities
{
    [Table("EvidenceImage")]
    public class EvidenceImage:BaseEntity
    {
        public string ImageUrl { set; get; }
        public int TaskEvidenceId { set; get; }

        [JsonIgnore]
        public virtual TaskEvidence TaskEvidence { get; set; }
    }
}

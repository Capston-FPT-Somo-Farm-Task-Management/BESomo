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
    [Table("TaskEvidence")]
    public class TaskEvidence:BaseEntity
    {
        public TaskEvidence()
        {
            EvidenceImages = new HashSet<EvidenceImage>();
        }

        [Required(ErrorMessage = "SubmitDate is required.")]
        public DateTime SubmitDate { get; set; }


        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        public int TaskId { get; set; }

        [JsonIgnore]
        public virtual ICollection<EvidenceImage> EvidenceImages { set; get; }
        [JsonIgnore]
        public virtual FarmTask? Task { set; get; }
    }
}

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
    public class TaskEvidence
    {
        public TaskEvidence()
        {
            EvidenceImages = new HashSet<EvidenceImage>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required]
        public int Status { set; get; }

        [Required(ErrorMessage = "SubmitDate is required.")]
        public DateTime SubmitDate { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        public int TaskId { get; set; }
        public  int? EvidenceType {  get; set; }

        [JsonIgnore]
        public virtual ICollection<EvidenceImage> EvidenceImages { set; get; }
        [JsonIgnore]
        public virtual FarmTask? Task { set; get; }
    }
}

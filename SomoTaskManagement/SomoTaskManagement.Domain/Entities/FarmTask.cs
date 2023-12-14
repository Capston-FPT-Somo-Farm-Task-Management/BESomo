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
    [Table("FarmTask")]
    public class FarmTask : BaseEntity
    {
        public FarmTask()
        {
            TaskEvidences = new HashSet<TaskEvidence>();
            Material_Tasks = new HashSet<Material_Task>();
            Employee_Tasks = new HashSet<Employee_Task>();
        }
        public string Code { get; set; }   
        public string? AddressDetail { get; set; }   
        [Required(ErrorMessage = "CreateDate is required.")]
        public DateTime CreateDate { set; get; }

       
        public DateTime? StartDate { set; get; }
        public DateTime? UpdateDate { set; get; }

      
        public DateTime? EndDate { set; get; }
        [Range(0, int.MaxValue, ErrorMessage = "OverallEfforMinutes must be greater than 0.")]
        public int? OverallEfforMinutes { set; get; }
        [Range(0, int.MaxValue, ErrorMessage = "OverallEffortHour must be greater than 0.")]
        public int? OverallEffortHour { set; get; }

        public string? Description { set; get; }

        public int? Priority { set; get; }
        public bool? IsRepeat { get; set; }
        public int? Remind { get; set; }

        public int? SuppervisorId { set; get; }
        public int? FieldId { set; get; }
        public int? TaskTypeId { set; get; }
        public int? ManagerId { set; get; }
        public int? PlantId { set; get; }
        public int? LiveStockId { set; get; }
        public bool? IsPlant {  get; set; }
        public bool IsSpecific {  get; set; }
        public int OriginalTaskId {  set; get; }
        public bool IsExpired {  set; get; }
        public bool? IsStartLate { set; get; } = null;
        public bool? IsImportant { set; get; } = null;

        [JsonIgnore]
        public virtual Member? Manager { set; get; }
        //[JsonIgnore]
        //public virtual Member? Supervisor { set; get; }
        [JsonIgnore]
        public virtual Plant? Plant { set; get; }
        [JsonIgnore]
        public virtual LiveStock? LiveStrock { set; get; }
        [JsonIgnore]
        public virtual Field? Field { set; get; }
        [JsonIgnore]
        public virtual TaskType? TaskType { set; get; }

        [JsonIgnore]
        public virtual ICollection<TaskEvidence> TaskEvidences { set; get; }
        [JsonIgnore]
        public virtual ICollection<Material_Task> Material_Tasks { set; get; }

        [JsonIgnore]
        public virtual ICollection<Employee_Task> Employee_Tasks { set; get; }

    }
}

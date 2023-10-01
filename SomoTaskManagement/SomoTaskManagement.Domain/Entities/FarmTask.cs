﻿using System;
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

        [Required(ErrorMessage = "CreateDate is required.")]
        public DateTime CreateDate { set; get; }

        [Required(ErrorMessage = "StartDate is required.")]
        public DateTime StartDate { set; get; }

        [Required(ErrorMessage = "EndDate is required.")]
        public DateTime EndDate { set; get; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { set; get; }


        [Required(ErrorMessage = "Priority is required.")]
        public int Priority { set; get; }
        public string Repeat { get; set; }
        public int Iterations { get; set; }
        public int Remind { get; set; }

        public int ReceiverId { set; get; }
        public int? FieldId { set; get; }
        public int TaskTypeId { set; get; }
        public int MemberId { set; get; }
        public int? OtherId { set; get; }
        public int? PlantId { set; get; }
        public int? LiveStockId { set; get; }

        [JsonIgnore]
        public virtual Member? Member { set; get; }
        [JsonIgnore]
        public virtual Plant? Plant { set; get; }
        [JsonIgnore]
        public virtual LiveStock? LiveStrock { set; get; }
        [JsonIgnore]
        public virtual Field? Field { set; get; }
        [JsonIgnore]
        public virtual Other? Other { set; get; }
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

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
    [Table("SubTask")]

    public class Employee_Task
    {
        [Key]
        public int TaskId { set; get; }
        [Key]
        public int EmployeeId { set; get; }

        public float? ActualEffort { get; set; }

        public string? Description { set; get; }

        public string? Name { set; get; }
        public bool? Status {  get; set; }
        [JsonIgnore]
        public virtual Employee? Employee { get; set; }
        [JsonIgnore]
        public virtual FarmTask? Task { get; set; }
    }
}

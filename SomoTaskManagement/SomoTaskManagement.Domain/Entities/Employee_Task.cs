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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int SubtaskId { get; set; } 
        
        public int TaskId { set; get; }
        
        public int EmployeeId { set; get; }

        public int ActualEfforMinutes { set; get; }
        public int ActualEffortHour { set; get; }
        public string? Code { get; set; }
        public DateTime? DaySubmit { get; set; }

        public string? Description { set; get; }

        public string? Name { set; get; }
        public bool? Status {  get; set; }
        [JsonIgnore]
        public virtual Employee? Employee { get; set; }
        [JsonIgnore]
        public virtual FarmTask? Task { get; set; }
    }
}

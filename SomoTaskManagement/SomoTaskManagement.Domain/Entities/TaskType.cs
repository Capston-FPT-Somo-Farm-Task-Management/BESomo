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
    [Table("TaskType")]

    public class TaskType:BaseEntity
    {
        public TaskType()
        {
            Tasks = new HashSet<FarmTask>();
            Employee_TaskTypes = new HashSet<Employee_TaskType>();
        }
        [Required(ErrorMessage = "Description is required.")]
        public string Description { set; get; }
        public bool IsDelete { set; get; }

        [JsonIgnore]
        public virtual ICollection<FarmTask> Tasks { get; set; }
        [JsonIgnore]
        public virtual ICollection<Employee_TaskType> Employee_TaskTypes { get; set; }    
    }
}

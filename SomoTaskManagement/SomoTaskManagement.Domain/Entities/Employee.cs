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
    [Table("Employee")]

    public class Employee:BaseEntity
    {
        public Employee()
        {
            Employee_Tasks = new HashSet<Employee_Task>();
            Employee_TaskTypes = new HashSet<Employee_TaskType>();
        }

        [StringLength(10, ErrorMessage = "Phone number must be exactly 10 digits.", MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must contain only digits.")]
        public string PhoneNumber { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }

        public int FarmId { get; set; }

        [JsonIgnore]
        public virtual Farm Farm { get; set; }
        [JsonIgnore]
        public virtual ICollection<Employee_Task> Employee_Tasks { set; get; }
        [JsonIgnore]
        public virtual ICollection<Employee_TaskType> Employee_TaskTypes { set; get; }

    }
}

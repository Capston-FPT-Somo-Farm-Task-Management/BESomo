using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Entities
{
    [Table("Employee_TaskType")]

    public class Employee_TaskType
    {
        [Key]
        public int TaskTypeId { set; get; }
        [Key]
        public int EmployeeId { set; get; }
        public bool Status { set; get; }

        public virtual TaskType? TaskType{set;get;}
        public virtual Employee? Employee{set;get;}
    }
}

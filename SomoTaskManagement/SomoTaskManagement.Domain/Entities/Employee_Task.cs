using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Entities
{
    [Table("Employee_Task")]

    public class Employee_Task
    {
        [Key]
        public int TaskId { set; get; }
        [Key]
        public int EmployeeId { set; get; }
        public bool Status { set; get; }

        public virtual Employee Employee { get; set; }
        public virtual FarmTask Task { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Employee
{
    public class EmployeeEffortInTask
    {
        public double Day { get; set; }

        public List<TaskEffort> TaskEfforts { get; set; }
    }
    public class TaskEffort
    {
        public string CodeTask { get; set; }
        public double EffortHour { get; set; }
    }
}

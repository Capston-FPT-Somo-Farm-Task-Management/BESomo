using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Employee
{
    public class EmployeeEffortUpdate
    {
        public int EmployeeId { get; set; }
        public int ActualEfforMinutes { set; get; }
        public int ActualEffortHour { set; get; }
    }
}

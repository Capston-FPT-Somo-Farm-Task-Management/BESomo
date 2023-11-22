using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.SubTask
{
    public class EmployeeEffortUpdateAndChangeStatus
    {
        public int EmployeeId { get; set; }
        public int ActualEfforMinutes { set; get; }
        public int ActualEffortHour { set; get; }
        public DateTime DaySubmit { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.SubTask
{
    public class TotalEffortModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public int ActualEfforMinutes { set; get; }
        public int ActualEffortHour { set; get; }
        public int TotalTask {  get; set; }
    }
}

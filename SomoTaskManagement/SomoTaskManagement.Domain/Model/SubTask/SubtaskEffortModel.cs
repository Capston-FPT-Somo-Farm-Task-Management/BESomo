using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.SubTask
{
    public class SubtaskEffortModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public int TotalActualEfforMinutes { set; get; }
        public int TotalActualEffortHour { set; get; }
        public DateTime? DaySubmit { get; set; }
        public string Code {  get; set; }
    }
}

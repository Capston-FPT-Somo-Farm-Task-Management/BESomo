using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.SubTask
{
    public class SubTaskModel
    {
        public int SubtaskId { get; set; } 
        public string TaskName { set; get; }
        public string CodeTask { set; get; }
        public int TaskId { set; get; }

        public string EmployeeName { set; get; }
        public string CodeEmployee { set; get; }
        public int EmployeeId { set; get; }
        public int ActualEfforMinutes { set; get; }
        public int ActualEffortHour { set; get; }

        public string? Description { set; get; }
        
       

        public string Name { set; get; }
        public DateTime? StartDay { get; set; }
        public DateTime? EndDay { get; set; }
        public string? Code { get; set; }
    }
}

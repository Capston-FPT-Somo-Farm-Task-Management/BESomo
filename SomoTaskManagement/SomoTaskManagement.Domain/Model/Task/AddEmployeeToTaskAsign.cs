using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Task
{
    public class AddEmployeeToTaskAsign
    {
        public List<int>? EmployeeIds {  get; set; }
        public int? OverallEfforMinutes {  get; set; }
        public int? OverallEffortHour {  get; set; }
    }
}

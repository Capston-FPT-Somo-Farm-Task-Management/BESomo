using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Task
{
    public class TaskByEmployeeDatesEffort
    {
        public IEnumerable<TaskByEmployeeDates> TaskByEmployeeDates { get; set; }
        public int  TotalPage {  get; set; }
    }
}

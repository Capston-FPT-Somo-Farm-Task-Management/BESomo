using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Task
{
    public class TotalTaskOfMonth
    {
        public int TaskCount { get; set; }
        public int? TotalTaskOfLivestock { get; set; }
        public int? TotalTaskOfPlant { get; set; }
        public int? TotalTaskOfOther { get; set; }
        public int? TotalTaskToDo { get; set; }
        public int? TotalTaskDoing { get; set; }
        public int? TotalTaskClose { get; set; }
        public int? TotalTaskPending { get; set; }
    }
}

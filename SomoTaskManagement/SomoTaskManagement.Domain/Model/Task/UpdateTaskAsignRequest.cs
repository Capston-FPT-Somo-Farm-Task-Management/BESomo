using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Task
{
    public class UpdateTaskAsignRequest
    {
        public List<int>? EmployeeIds { get; set; }
        public List<int>? MaterialIds { get; set; }
        public TaskUpdateAsignModel TaskModel { get; set; }
    }
}

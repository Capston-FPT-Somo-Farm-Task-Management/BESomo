using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model
{
    public class TaskRequestModel
    {
        public List<int> EmployeeIds { get; set; }
        public List<int> MaterialIds { get; set; }
        public TaskCreateUpdateModel FarmTask { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Task
{
    public class TaskRequestModel
    {
        public List<int> EmployeeIds { get; set; }
        public List<int> MaterialIds { get; set; }
        [FromQuery]
        public List<DateTime>? Dates { get; set; } = null;
        public TaskCreateUpdateModel FarmTask { get; set; }
    }
}

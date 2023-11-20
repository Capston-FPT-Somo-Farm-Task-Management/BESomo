using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Task
{
    public class TaskDraftRequest
    {
        public List<DateTime>? Dates { get; set; } = null;
        public List<int>? MaterialIds { get; set; } = null;
        public TaskDraftModel FarmTask { get; set; }
    }
}

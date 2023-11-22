using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Task
{
    public class UpdateTaskDraftAndToPrePare
    {
        public List<DateTime>? Dates { get; set; }
        public List<int>? MaterialIds { get; set; }
        public TaskDraftModelUpdate TaskModel { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Task
{
    public class RequestUpdateTaskDraft
    {
        public TaskDraftModelUpdate TaskModel {  get; set; }
        public List<DateTime>? Dates {  get; set; }
        public List<int> MaterialIds {  get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.SubTask
{
    public class GetEffortByTaskModel
    {
         public IEnumerable<SubtaskEffortModel> Subtasks { get; set; }
        public bool IsHaveSubtask {  get; set; }
    }
}

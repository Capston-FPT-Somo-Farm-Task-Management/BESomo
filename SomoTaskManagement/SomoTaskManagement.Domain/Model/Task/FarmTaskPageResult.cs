using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Task
{
    public class FarmTaskPageResult
    {
        public IEnumerable<FarmTaskModel> FarmTasks { get; set; }
        public int TotalPages { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Task
{
    public class CreateAsignTaskRequest
    {
       public TaskCreateAsignModel TaskModel {  get; set; }
        public List<int>? MaterialIds {  get; set; }
        public List<int>? EmployeeIds {  get; set; }
    }
}

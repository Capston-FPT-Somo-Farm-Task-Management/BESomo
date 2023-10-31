using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Employee
{
    public class EmployeeUpdateRequest
    {
        public EmployeeCreateModel EmployeeCreateModel {  get; set; }
        public List<int> TaskTypeIds {  get; set; }
    }
}

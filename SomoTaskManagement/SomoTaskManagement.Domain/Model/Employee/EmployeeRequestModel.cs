using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Employee
{
    public class EmployeeRequestModel
    {
        public List<int> TaskTypeId { get; set; }
        public EmployeeCreateModel Employee { get; set; }
    }
}

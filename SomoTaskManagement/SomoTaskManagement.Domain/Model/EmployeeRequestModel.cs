using SomoTaskManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model
{
    public class EmployeeRequestModel
    {
        public List<int> TaskTypeId { get; set; }
        public Employee Employee { get; set; }
    }
}

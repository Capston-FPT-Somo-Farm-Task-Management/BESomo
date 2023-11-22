using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Employee
{
    public class EmployeeEffortInMonthModel
    {
        public int Day { get; set; }
        public DateTime? EndDate { get; set; }
        public double EffortHour { get; set; }
    }
}

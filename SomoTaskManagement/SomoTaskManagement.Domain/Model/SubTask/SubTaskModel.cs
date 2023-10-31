using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.SubTask
{
    public class SubTaskModel
    {
        public string TaskName { set; get; }
        public int TaskId { set; get; }

        public string EmployeeName { set; get; }
        public int EmployeeId { set; get; }

        public int? ActualEffort { get; set; }

        public string? Description { set; get; }

        public string Name { set; get; }
    }
}

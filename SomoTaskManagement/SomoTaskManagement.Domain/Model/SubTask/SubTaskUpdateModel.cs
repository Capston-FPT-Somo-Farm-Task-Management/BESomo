﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.SubTask
{
    public class SubTaskUpdateModel
    {
        public int EmployeeId { set; get; }
        public string? Description { set; get; }
        public DateTime? DaySubmit { get; set; }
        public string Name { set; get; }
        public int OverallEfforMinutes { set; get; }
        public int OverallEffortHour { set; get; }
    }
}

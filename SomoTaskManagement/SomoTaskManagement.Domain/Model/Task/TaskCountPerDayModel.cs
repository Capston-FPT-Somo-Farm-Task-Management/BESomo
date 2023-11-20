﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Task
{
    public class TaskCountPerDayModel
    {
        public DateTime? Date { get; set; }
        public int TaskCount { get; set; }
        public int? TotalTaskOfLivestock { get; set; }
        public int? TotalTaskOfPlant { get; set; }
        public int? TotalTaskOfOther { get; set; }
    }
}

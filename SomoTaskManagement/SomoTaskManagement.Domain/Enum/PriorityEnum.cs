using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Enum
{
    public enum PriorityEnum
    {
        [Description("Thấp nhất")]
        Shortest = 0,

        [Description("Thấp")]
        Short = 1,

        [Description("Trung bình")]
        Medium = 2, 

        [Description("Cao")]
        High = 3,

        [Description("Cao nhất")]
        Tallest = 4,
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Enum
{
    public enum PlantLivestockEnum
    {
        [Description("Công việc cây trồng")]
        TaskPlant = 0,
        [Description("Công việc chăn nuôi")]
        TaskLivestock = 1,
        [Description("Công việc khác")]
        TaskOther = 2,
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Enum
{
    public enum HabitantTypeStatus
    {
        [Description("Thực vật")]
        Plant = 0,
        [Description("Động vật")]
        LiveStock = 1,
        [Description("Xóa")]
        IsDelete = 2
    }
}







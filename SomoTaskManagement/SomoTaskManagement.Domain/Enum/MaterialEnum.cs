using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Enum
{
    public enum MaterialEnum
    {

        [Description("Bị hư")]
        Broken = 0,
        [Description("Đang sử dụng")]
        Used = 1
    }
}

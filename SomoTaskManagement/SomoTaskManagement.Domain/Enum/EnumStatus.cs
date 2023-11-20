using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Enum
{
    public enum EnumStatus
    {
        [Description("Ẩn")]
        Inactive = 0,
        [Description("Hiện")]
        Active = 1
    }
}

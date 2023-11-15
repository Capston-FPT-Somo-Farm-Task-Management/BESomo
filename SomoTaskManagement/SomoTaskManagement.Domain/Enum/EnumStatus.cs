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
        [Description("Không tồn tại")]
        Inactive = 0,
        [Description("Tồn tại")]
        Active = 1
    }
}

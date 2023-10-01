using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Enum
{
    public enum TaskStatusEnum
    {
        [Description("Không hoàn thành")]
        Unfinished = 0,

        [Description("Chuẩn bị")]
        Prepare = 1,

        [Description("Đang thực hiện")]
        Processing = 2,

        [Description("Hoàn thành")]
        Complete = 3,

        [Description("Hoàn thành")]
        IsDelete = 4,
    }
}

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

        [Description("Chuẩn bị")]
        Prepare = 0,

        [Description("Đang thực hiện")]
        Processing = 1,

        [Description("Hoàn thành")]
        Complete = 2,

        [Description("Không hoàn thành")]
        Unfinished = 3,

        [Description("Đã xóa")]
        IsDelete = 4,
    }
}

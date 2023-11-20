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
        [Description("Bản nháp")]
        Draft = 0,

        [Description("Chuẩn bị")]
        ToDo = 1,

        [Description("Đã giao")]
        Assigned = 2,

        [Description("Đang thực hiện")]
        Doing = 3,

        [Description("Hoàn thành")]
        Done = 4,

        [Description("Tạm hoãn")]
        Pending = 5,

        [Description("Từ chối")]
        Rejected = 6,

        [Description("Hủy bỏ")]
        Cancelled = 7,

        [Description("Đã đóng")]
        Closed = 8,
    }

}

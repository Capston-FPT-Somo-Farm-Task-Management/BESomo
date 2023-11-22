using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Enum
{
    public enum EvidenceTypeEnum
    {
        [Description("Bình thường")]
        Normal = 0,

        [Description("Không đồng ý")]
        ToDo = 1,

        [Description("Hủy bỏ")]
        Cancelled = 2,

        [Description("Tạm hoãn")]
        Pending = 3,

        [Description("Đang thực hiện")]
        Doing = 4,

        [Description("Từ chối người giám sát")]
        Refuse = 5,
    }
}

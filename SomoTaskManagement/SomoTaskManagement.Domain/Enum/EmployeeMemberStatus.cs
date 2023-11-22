using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Enum
{
    public enum EmployeeMemberStatus
    {
        [Description("Đang làm việc")]
        Working = 1,
        [Description("Đã nghỉ việc")]
        Quit = 0
    }
}

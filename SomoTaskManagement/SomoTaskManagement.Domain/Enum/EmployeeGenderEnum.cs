using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Enum
{
    public enum EmployeeGenderEnum
    {
        [Description("Nam")]
        Male = 0,
        [Description("Nữ")]
        Female = 1,
    }
}

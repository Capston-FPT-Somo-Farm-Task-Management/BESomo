using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.TaskType
{
    public class TaskTypeImportModel
    {
        public IFormFile ExcelFile { get; set; }
    }
}

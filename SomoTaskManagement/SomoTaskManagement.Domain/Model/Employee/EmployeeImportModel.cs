using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Employee
{
    public class EmployeeImportModel
    {
        //[Required(ErrorMessage = "Nhập thiếu file excel")]
        public IFormFile ExcelFile { get; set; }
    }
}

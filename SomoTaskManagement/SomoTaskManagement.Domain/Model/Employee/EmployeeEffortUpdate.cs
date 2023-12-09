using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Employee
{
    public class EmployeeEffortUpdate
    {
        [Required(ErrorMessage ="Vui lòng chọn nhân viên")]
        public int EmployeeId { get; set; }
        public int ActualEfforMinutes { set; get; }
        public int ActualEffortHour { set; get; }
        [Required(ErrorMessage = "Ngày bắt buộc nhập")]
        public DateTime DaySubmit {  get; set; }
        [Required(ErrorMessage = "Mô tả bắt buộc nhập")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Tên nhiệm vụ bắt buộc nhập")]
        public string Name { get; set; }
    }
}

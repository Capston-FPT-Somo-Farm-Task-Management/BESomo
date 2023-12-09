using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Employee
{
    public class EmployeeCreateModel
    {

        [Required(ErrorMessage = "Tên bắt buộc nhập")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Mã nhân viên bắt buộc nhập")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Số điện thoại bắt buộc nhập")]
        [StringLength(10, ErrorMessage = "Số điện thoại không hợp lệ", MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Không được chứa chữ và kí tự đặc biệt")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Địa chỉ bắt buộc nhập")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được quá 200 kí tự")]
        public string Address { get; set; }
        [Required]
        public bool Gender { get; set; }
        [Required]
        public IFormFile? ImageFile { get; set; }
        [Required]
        public int FarmId { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public List<int> TaskTypeIds {  get; set; }
    }
}

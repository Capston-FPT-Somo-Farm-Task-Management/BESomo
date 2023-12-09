using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Member
{
    public class MemberCreateUpdateModel
    {
        [Required]
        [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Tên không được chứa kí tự đặc biệt và số")]
        [StringLength(100, ErrorMessage = "Tên không được quá 100 kí tự")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email bắt buộc nhập")]
        public string Code { get; set; }
        //[Required]
        //public int Status { set; get; }
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Required(ErrorMessage = "Email bắt buộc nhập")]
        public string Email { set; get; }

        [StringLength(100, ErrorMessage = "Tên đăng nhập không được quá 100 kí tự")]
        [Required(ErrorMessage = "Tên đăng nhập bắt buộc nhập")]
        public string UserName { set; get; }

        [Required(ErrorMessage = "Password is required.")]
        //[RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@#$%^&+=!])(?!.*\s).{8,}$",
        //ErrorMessage = "Password must meet the complexity requirements.")]
        public string Password { set; get; }

        [StringLength(10, ErrorMessage = "Số điện thoại không hợp lệ.", MinimumLength = 10)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Số điện thoại chỉ được chứa số")]
        public string PhoneNumber { set; get; }


        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        //[Range(typeof(DateTime), "1900-01-01", "2005-01-01", ErrorMessage = "Please enter a valid date.")]
        [Required(ErrorMessage = "Birthday is required.")]
        public DateTime Birthday { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        [Required(ErrorMessage = "Address is required.")]
        public string Address { set; get; }
        public int RoleId { set; get; }
        public int FarmId { set; get; }
        public IFormFile ImageFile { get; set; }
    }
}

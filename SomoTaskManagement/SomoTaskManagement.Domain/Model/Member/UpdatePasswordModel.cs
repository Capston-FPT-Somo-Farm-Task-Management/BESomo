using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.Member
{
    public class UpdatePasswordModel
    {
        [Required(ErrorMessage ="Mật khẩu cũ bắt buộc nhập")]
        public string OldPassword {  get; set; }

        [Required(ErrorMessage = "Mật khẩu mới bắt buộc nhập")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu bắt buộc nhập")]
        public string ConfirmPassword { get; set; }
    }
}

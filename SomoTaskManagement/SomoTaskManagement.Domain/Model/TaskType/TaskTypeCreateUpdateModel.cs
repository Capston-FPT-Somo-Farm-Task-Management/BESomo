using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.TaskType
{
    public class TaskTypeCreateUpdateModel
    {

        [Required(ErrorMessage = "Tên bắt buộc nhập")]
        [RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "Name must contain only letters and spaces.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Trạng thái bắt buộc nhập")]
        public int Status { set; get; }

        [Required(ErrorMessage = "Mô tả bắt buộc nhập")]
        public string Description { set; get; }
    }
}

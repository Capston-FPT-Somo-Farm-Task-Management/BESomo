using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomoTaskManagement.Domain.Model.HabitantType
{
    public class HabitantTypeCUModel
    {
        [Required(ErrorMessage = "Tên bắt buộc nhập")]
        //[RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "Name must contain only letters and spaces.")]
        [StringLength(100, ErrorMessage = "Tên không được quá 100 kí tự")]
        public string Name { get; set; }
        public string? Origin { get; set; }
        public string? Environment { get; set; }
        public string? Description { get; set; }
        [Required]
        public int Status { set; get; }
        public int FarmId { set; get; }
    }
}
